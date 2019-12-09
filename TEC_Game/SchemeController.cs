using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using tec;

namespace TEC_Game
{
    class SchemeController
    {
        private GameController gameController;

        public SchemeController(GameController controller)
        {
            gameController = controller;
        }

        public void PlaceNode(ref string line)
        {
            int id = Int32.Parse(GetSubString(ref line, line.IndexOf(' '))); //id к=узла

            int row = Int32.Parse(GetSubString(ref line, line.IndexOf(' '))); //номер строки и столбца в grid для узла
            int column = Int32.Parse(GetSubString(ref line, line.Length));

            Node node = new Node(id, column, row);

            gameController.scheme.AddNode(node); //Добавление узла в схему

            node.Template = gameController.gameWindow.FindResource("NodeTemplate") as ControlTemplate; //Задания шаблона для узла

            node.Click += new RoutedEventHandler(gameController.OnNodeClick); //Добавление обработчика нажатия

            node.Content = id.ToString(); //Задание текста на узле

            node.SetValue(Grid.RowProperty, row); //Задание положения узла в grid
            node.SetValue(Grid.ColumnProperty, column);
            node.SetValue(Panel.ZIndexProperty, 2);

            gameController.gameWindow.GameGrid.Children.Add(node); //Добавление узла в grid
        }

        public void PlaceElement(ref string line, string type)
        {
            int id = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));

            int row = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));
            int column = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));

            string direction = GetSubString(ref line, 1);

            int node1Id = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));
            int node2Id = Int32.Parse(GetSubString(ref line, line.Length));

            Node node1 = gameController.scheme.GetNode(node1Id);
            Node node2 = gameController.scheme.GetNode(node2Id);

            BaseElement element;

            switch (type)
            {
                case "Re":
                    element = new Resistor(node1, node2, id);
                    break;
                case "No":
                    element = new Norator(node1, node2, id);
                    break;
                case "Nu":
                    element = new Nullator(node1, node2, id);
                    break;
                default:
                    element = new Conductor(node1, node2, id);
                    break;

            }

            gameController.scheme.AddElement(element);

            element.GetImage().SetValue(Grid.RowProperty, row);
            element.GetImage().SetValue(Grid.ColumnProperty, column);
            element.GetImage().SetValue(Panel.ZIndexProperty, 1);

            if (direction == "R")
            {
                element.ChangeImageDirectionToLand();

                element.GetImage().SetValue(Grid.RowSpanProperty, 3);
                element.GetImage().SetValue(Grid.ColumnSpanProperty, 9);
            }
            else
            {
                element.GetImage().SetValue(Grid.RowSpanProperty, 9);
                element.GetImage().SetValue(Grid.ColumnSpanProperty, 3);
            }

            gameController.gameWindow.GameGrid.Children.Add(element.GetImage());
        }

        public void PlaceWire(ref string line)
        {
            int id = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));

            int startRow = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));
            int startColumn = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));

            int length = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));

            string direction = GetSubString(ref line, 1);

            int id1 = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));
            string type1 = GetSubString(ref line, 1);
            object obj1 = null;
            if (type1 == "N")
            {
                obj1 = gameController.scheme.GetNode(id1);
            }
            else
            {
                obj1 = gameController.scheme.GetElement(id1);
            }

            object obj2 = null;
            if (line != "")
            {
                int id2 = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));
                string type2 = GetSubString(ref line, 1);
                if (type2 == "N")
                {
                    obj2 = gameController.scheme.GetNode(id2);
                }
                else
                {
                    obj2 = gameController.scheme.GetElement(id2);
                }
            }

            Wire wire = new Wire(id, startRow, startColumn);

            gameController.scheme.AddWireToObject(wire, obj1);
            gameController.scheme.AddWireToObject(wire, obj2);

            gameController.scheme.AddWire(wire);

            wire.GetImage().SetValue(Grid.RowProperty, startRow);
            wire.GetImage().SetValue(Grid.ColumnProperty, startColumn);
            wire.GetImage().SetValue(Panel.ZIndexProperty, 1);
            

            if (direction == "R")
            {
                wire.ChangeImageDirectionToLand();
                wire.GetImage().SetValue(Grid.ColumnSpanProperty, length);
            }
            else
            {
                wire.GetImage().SetValue(Grid.RowSpanProperty, length);
            }
            
            gameController.gameWindow.GameGrid.Children.Add(wire.GetImage());
            
            if (direction == "R")
            {
                
                double marginDistance = gameController.gameWindow.GameGrid.ActualWidth / 70 / 2;
                wire.GetImage().Margin = new Thickness(marginDistance, 0, marginDistance, 0); //Попытка добавить marginProperty к ячейке, в которой записана картинка
            }
            else
            {
                double marginDistance = gameController.gameWindow.GameGrid.ActualHeight / 50 / 2; 
                wire.GetImage().Margin = new Thickness(0, marginDistance, 0, marginDistance); //Попытка добавить marginProperty к ячейке, в которой записана картинка
            }
        }

        public string GetSubString(ref string line, int len)
        {
            string ans = line.Substring(0, len);
            line = line.Length <= len + 1 
                 ? "" 
                 : line.Substring(len + 1);
            return ans;
        }

        public void FindPlaceAndCreateNullor(Node node1, Node node2, string type)
        {
            string line = "";
            int row = 0;
            int column = 0;
            int id = gameController.scheme.GetElementMaxId() + 1;
            string direction = "";

            var blockingElements = FindBlockingElements(node1, node2, ref direction);
            blockingElements.Remove(node1);
            blockingElements.Remove(node2);

            Norator norator = null;
            Nullator nullator = null;
            foreach (var elem in blockingElements)
            {
                if (elem is Norator) norator = (Norator) elem;
                if (elem is Nullator) nullator = (Nullator) elem;
            }

            blockingElements.Remove(norator);
            blockingElements.Remove(nullator);

            row = direction == "U" ? node2.GetY() : node1.GetY();
            column = direction == "L" ? node2.GetX() : node1.GetX();

            if (!blockingElements.Any())
            {
                line = (direction == "R") || (direction == "L")
                    ? id + " " + (row - 1) + " " + column + " R " + node1.GetId() + " " + node2.GetId()
                    : id + " " + row + " " + (column - 1) + " D " + node1.GetId() + " " + node2.GetId();

                PlaceElement(ref line, type);

                int length = (direction == "R") || (direction == "L")
                    ? Math.Abs(node1.GetX() - node2.GetX())
                    : Math.Abs(node1.GetY() - node2.GetY());

                //Если элемент слишком короткий(т.е. он не дотягивается до второго узла), то лучше пририсовать ему провод, чтобы нормально все выглядело
                if (length > 8)
                {
                    int wireId = gameController.scheme.GetWireMaxId();

                    int id1 = direction == "U" || direction == "L" ? node2.GetId() : node1.GetId();

                    line = (direction == "R") || (direction == "L") 
                        ? wireId + " " + row + " " + (column + 8) + " " + (length - 7) + " R " + id1 + " N " + id + " E"
                        : wireId + " " + (row + 8) + " " + column + " " + (length - 7) + " D " + id1 + " N " + id + " E";

                    PlaceWire(ref line);
                    line = "";
                }

                //Если у узлов не совпадают ни X, ни Y (здесь нужен будет еще один провод)
                if ((node1.GetX() != node2.GetX()) && (node1.GetY() != node2.GetY()))
                {
                    int wireId = gameController.scheme.GetWireMaxId();

                    int wireRow = 0;
                    int wireColumn = 0;

                    if ((direction == "R") || (direction == "L"))
                    {
                        wireRow = node2.GetY() > node1.GetY() ? node1.GetY() : node2.GetY();
                        wireColumn = node2.GetX();
                    }
                    else
                    {
                        wireRow = node2.GetY();
                        wireColumn = node2.GetX() > node1.GetX() ? node1.GetX() : node2.GetX();
                    }

                    length = (direction == "R") || (direction == "L") 
                             ? Math.Abs(node1.GetY() - node2.GetY())
                             : Math.Abs(node1.GetX() - node2.GetX());

                    line = (direction == "R") || (direction == "L")
                        ? (wireId + 1) + " " + wireRow + " " + wireColumn + " " + length + " D " + node1.GetId() + " N " + node2.GetId() + " N"
                        : (wireId + 1) + " " + wireRow + " " + wireColumn + " " + length + " R " + node1.GetId() + " N " + node2.GetId() + " N";

                    PlaceWire(ref line);
                    line = "";

                    line = (direction == "R") || (direction == "L")
                        ? (gameController.scheme.GetNodeMaxId() + 1) + " " + node1.GetY() + " " + node2.GetX()
                        : (gameController.scheme.GetNodeMaxId() + 1) + " " + node2.GetY() + " " + node1.GetX();
                    PlaceNode(ref line);
                    node2 = gameController.scheme.GetNode(gameController.scheme.GetNodeMaxId());
                }
            }
            else if (blockingElements.Count == 1)
            {
                //Если есть блокирующие элементы и нужно как-то считать положение элемента
                //TO DO
                if (direction == "R" && node1.GetY() > node2.GetY())
                {
                    var temp = node1;
                    node1 = node2;
                    node2 = temp;
                }
                if (direction == "D" && node1.GetX() > node2.GetX())
                {
                    var temp = node1;
                    node1 = node2;
                    node2 = temp;
                }
                direction = node1.GetY() == node2.GetY() ? "R" : "D";
                id = gameController.scheme.GetElementMaxId() + 1;
                line = direction == "R" 
                    ? id + " " + (node1.GetY() - 4) + " " + (node1.GetX() + 4) + " R " + node1.GetId() + " " + node2.GetId()
                    : id + " " + (node1.GetY() + 4) + " " + (node1.GetX() + 2) + " D " + node1.GetId() + " " + node2.GetId();
                PlaceElement(ref line, type);

                int wireId = gameController.scheme.GetWireMaxId() + 1;
                line = direction == "R"
                    ? wireId + " " + (node1.GetY() - 3) + " " + node1.GetX() + " 6 R " + id + " E"
                    : wireId + " " + node1.GetY() + " " + (node1.GetX() + 3) + " 6 D " + id + " E";
                PlaceWire(ref line);

                Wire wire1 = gameController.scheme.GetWire(wireId);

                wireId++;
                line = direction == "R"
                    ? wireId + " " + (node1.GetY() - 3) + " " + (node1.GetX() + 12) + " 6 R " + id + " E"
                    : wireId + " " + (node1.GetY() + 12) + " " + (node1.GetX() + 3) + " 6 D " + id + " E";
                PlaceWire(ref line);

                Wire wire2 = gameController.scheme.GetWire(wireId);

                wireId++;
                line = direction == "R"
                    ? wireId + " " + (node1.GetY() - 3) + " " + node1.GetX() + " 4 D " + node1.GetId() + " N"
                    : wireId + " " + node1.GetY() + " " + node1.GetX() + " 4 R " + node1.GetId() + " N";
                PlaceWire(ref line);

                Wire wire3 = gameController.scheme.GetWire(wireId);
                gameController.scheme.AddWireToObject(wire1, wire3);

                wireId++;
                line = direction == "R"
                    ? wireId + " " + (node2.GetY() - 3) + " " + node2.GetX() + " 4 D " + node2.GetId() + " N"
                    : wireId + " " + node2.GetY() + " " + node2.GetX() + " 4 R " + node2.GetId() + " N";
                PlaceWire(ref line);

                Wire wire4 = gameController.scheme.GetWire(wireId);
                gameController.scheme.AddWireToObject(wire2, wire4);
            }
        }

        private HashSet<object> FindBlockingElements(Node node1, Node node2, ref string direction)
        {
            HashSet<object> ans = new HashSet<object>();
            HashSet<object> tempList = new HashSet<object>();
            int x = node1.GetX();
            int y = node1.GetY();
            int xStep = node1.GetX() - node2.GetX() > 0 ? -1 : 1;
            int yStep = node1.GetY() - node2.GetY() > 0 ? -1 : 1;

            if (x != node2.GetX())
            {
                while (x != node2.GetX())
                {
                    if (gameController.scheme.GetNode(x, y) != null)
                    {
                        if (gameController.scheme.GetNode(x, y) != node2)
                            ans.Add(gameController.scheme.GetNode(x, y));
                        if (FindHorizontalElement(gameController.scheme.GetNode(x, y), xStep).Count != 0)
                        {
                            HashSet<BaseElement> set = FindHorizontalElement(gameController.scheme.GetNode(x, y), xStep);
                            foreach (var element in set)
                            {
                                tempList.Add(element);
                            }
                        }
                    }

                    x += xStep;
                }

                while (y != node2.GetY())
                {
                    if (gameController.scheme.GetNode(x, y) != null)
                    {
                        if (gameController.scheme.GetNode(x, y) != node2)
                            ans.Add(gameController.scheme.GetNode(x, y));
                        if (FindVerticalElement(gameController.scheme.GetNode(x, y), yStep).Count != 0)
                        {
                            HashSet<BaseElement> set = FindVerticalElement(gameController.scheme.GetNode(x, y), yStep);
                            foreach (var element in set)
                            {
                                tempList.Add(element);
                            }
                        }
                    }

                    y += yStep;
                }

                if (!ans.Any())
                {
                    if (xStep < 0)
                        direction = "L";
                    else
                        direction = "R";
                    return ans;
                }
            }

            x = node1.GetX();
            y = node1.GetY();

            while (y != node2.GetY())
            {
                if (gameController.scheme.GetNode(x, y) != null)
                {
                    if (gameController.scheme.GetNode(x, y) != node2)
                        tempList.Add(gameController.scheme.GetNode(x, y));
                    if (FindVerticalElement(gameController.scheme.GetNode(x, y), yStep).Count != 0)
                    {
                        HashSet<BaseElement> set = FindVerticalElement(gameController.scheme.GetNode(x, y), yStep);
                        foreach (var element in set)
                        {
                            tempList.Add(element);
                        }
                    }
                }

                y += yStep;
            }

            while (x != node2.GetX())
            {
                if (gameController.scheme.GetNode(x, y) != null)
                {
                    if (gameController.scheme.GetNode(x, y) != node2)
                        tempList.Add(gameController.scheme.GetNode(x, y));
                    if (FindHorizontalElement(gameController.scheme.GetNode(x, y), xStep).Count != 0)
                    {
                        HashSet<BaseElement> set = FindHorizontalElement(gameController.scheme.GetNode(x, y), xStep);
                        foreach (var element in set)
                        {
                            tempList.Add(element);
                        }
                    }
                }

                x += xStep;
            }

            if (yStep < 0)
                direction = "U";
            else
                direction = "D";
            return tempList;
        }

        private HashSet<BaseElement> FindVerticalElement(Node node, int yStep)
        {
            Node temp = gameController.scheme.GetVerticalNode(node, yStep);
            HashSet<BaseElement> ans = new HashSet<BaseElement>();

            foreach (var element in node.GetConnectedElements())
                if ((element.GetNode1() == temp) || (element.GetNode2() == temp))
                    ans.Add(element);
            return ans;
        }

        private HashSet<BaseElement> FindHorizontalElement(Node node, int xStep)
        {
            Node temp = gameController.scheme.GetHorizontalNode(node, xStep);
            HashSet<BaseElement> ans = new HashSet<BaseElement>();

            foreach (var element in node.GetConnectedElements())
                if ((element.GetNode1() == temp) || (element.GetNode2() == temp))
                    ans.Add(element);
            return ans;
        }
    }
}
