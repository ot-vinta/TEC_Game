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

            Wire wire = new Wire(id, startRow, startColumn);

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
                
                double marginDistance = gameController.gameWindow.GameGrid.ActualWidth / 40 / 2;
                wire.GetImage().Margin = new Thickness(marginDistance, 0, marginDistance, 0); //Попытка добавить marginProperty к ячейке, в которой записана картинка
            }
            else
            {
                double marginDistance = gameController.gameWindow.GameGrid.ActualHeight / 40 / 2; 
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
            int id = gameController.scheme.GetElementsSize();
            string direction = "";

            var blockingElements = FindBlockingElements(node1, node2, ref direction);

            row = direction == "U" ? node2.GetY() : node1.GetY();
            column = direction == "L" ? node2.GetX() : node1.GetX();

            if (!blockingElements.Any())
            {
                int length = (direction == "R") || (direction == "L")
                    ? Math.Abs(node1.GetX() - node2.GetX())
                    : Math.Abs(node1.GetY() - node2.GetY());

                //Если элемент слишком короткий(т.е. он не дотягивается до второго узла), то лучше пририсовать ему провод, чтобы нормально все выглядело
                if (length > 8)
                {
                    int wireId = gameController.scheme.GetWiresCount();

                    line = (direction == "R") || (direction == "L") 
                        ? wireId + " " + row + " " + (column + 8) + " " + (length - 7) + " R"
                        : wireId + " " + (row + 8) + " " + column + " " + (length - 7) + " D";

                    PlaceWire(ref line);
                    line = "";
                }

                //Если у узлов не совпадают ни X, ни Y (здесь нужен будет еще один провод)
                if ((node1.GetX() != node2.GetX()) && (node1.GetY() != node2.GetY()))
                {
                    int wireId = gameController.scheme.GetWiresCount();

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
                        ? (wireId + 1) + " " + wireRow + " " + wireColumn + " " + length + " D"
                        : (wireId + 1) + " " + wireRow + " " + wireColumn + " " + length + " R";

                    PlaceWire(ref line);
                    line = "";

                    line = (direction == "R") || (direction == "L")
                        ? (gameController.scheme.GetNodesCount() + 1) + " " + node1.GetY() + " " + node2.GetX()
                        : (gameController.scheme.GetNodesCount() + 1) + " " + node2.GetY() + " " + node1.GetX();
                    PlaceNode(ref line);
                    node2 = gameController.scheme.GetNode(gameController.scheme.GetNodesCount());
                }

                line = (direction == "R") || (direction == "L")
                    ? (id + 1) + " " + (row - 1) + " " + column + " R " + node1.GetId() + " " + node2.GetId()
                    : (id + 1) + " " + row + " " + (column - 1) + " D " + node1.GetId() + " " + node2.GetId();

                PlaceElement(ref line, type);
            }
            else if (blockingElements.Count == 1 && 
                     (node1.GetNullorElement() == null || node2.GetNullorElement() == null))
            {
                //Если есть блокирующие элементы и нужно как-то считать положение элемента
                //TO DO
                /*direction = node1.GetX() == node2.GetX() ? "U" : "R";
                id = gameController.scheme.GetElementsSize() + 1;
                line = direction == "U" 
                    ? id + " " + (node1.GetY() - 4) + " " + (node1.GetX() + 4) + " R " + node1.GetId() + " " + node2.GetId()
                    : id + " " + (node1.GetY() + 4) + " " + (node1.GetX() + 4) + " D " + node1.GetId() + " " + node2.GetId();
                PlaceElement(ref line, type);

                id = gameController.scheme.GetWiresCount() + 1;
                line = direction == "U"
                    ? id + " " + node1.GetX() + " " + (node1.GetY() - 3) + " 6 R"
                    : id + " " + (node1.GetX() + 3) + " " + node1.GetY() + " 6 D";
                PlaceWire(ref line);
                id++;
                line = direction == "U"
                    ? id + " " + (node1.GetX() + ) + " " + (node1.GetY() - 3) + " 6 R"
                    : id + " " + (node1.GetX() + 3) + " " + node1.GetY() + " 6 D";
                PlaceWire(ref line);*/
            }
        }

        private List<object> FindBlockingElements(Node node1, Node node2, ref string direction)
        {
            List<object> ans = new List<object>();
            List<object> tempList = new List<object>();
            int x = node1.GetX();
            int y = node1.GetY();
            int xStep = node1.GetX() - node2.GetX() > 0 ? -1 : 1;
            int yStep = node1.GetY() - node2.GetY() > 0 ? -1 : 1;

            if (x != node2.GetX())
            {
                while (x != node2.GetX())
                {
                    x += xStep;

                    if (gameController.scheme.GetNode(x, y) != null)
                    {
                        if (gameController.scheme.GetNode(x, y) != node2)
                            ans.Add(gameController.scheme.GetNode(x, y));
                        if (FindHorizontalElement(gameController.scheme.GetNode(x, y), xStep) != null)
                            ans.Add(FindHorizontalElement(gameController.scheme.GetNode(x, y), xStep));
                    }
                }

                while (y != node2.GetY())
                {
                    y += yStep;

                    if (gameController.scheme.GetNode(x, y) != null)
                    {
                        if (gameController.scheme.GetNode(x, y) != node2)
                            ans.Add(gameController.scheme.GetNode(x, y));
                        if (FindVerticalElement(gameController.scheme.GetNode(x, y), yStep) != null)
                            ans.Add(FindVerticalElement(gameController.scheme.GetNode(x, y), yStep));
                    }
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
                y += yStep;

                if (gameController.scheme.GetNode(x, y) != null)
                {
                    if (gameController.scheme.GetNode(x, y) != node2)
                        tempList.Add(gameController.scheme.GetNode(x, y));
                    if (FindVerticalElement(gameController.scheme.GetNode(x, y), yStep) != null)
                        tempList.Add(FindVerticalElement(gameController.scheme.GetNode(x, y), yStep));
                }
            }

            while (x != node2.GetX())
            {
                x += xStep;

                if (gameController.scheme.GetNode(x, y) != null)
                {
                    if (gameController.scheme.GetNode(x, y) != node2)
                        tempList.Add(gameController.scheme.GetNode(x, y));
                    if (FindHorizontalElement(gameController.scheme.GetNode(x, y), xStep) != null)
                        tempList.Add(FindHorizontalElement(gameController.scheme.GetNode(x, y), xStep));
                }
            }

            if (yStep < 0)
                direction = "U";
            else
                direction = "D";
            return tempList;
        }

        private BaseElement FindVerticalElement(Node node, int yStep)
        {
            Node temp = gameController.scheme.GetVerticalNode(node, yStep);

            foreach (var element in node.GetConnectedElements())
                if ((element.GetNode1() == temp) || (element.GetNode2() == temp))
                    return element;
            return null;
        }

        private BaseElement FindHorizontalElement(Node node, int xStep)
        {
            Node temp = gameController.scheme.GetHorizontalNode(node, xStep);

            foreach (var element in node.GetConnectedElements())
                if ((element.GetNode1() == temp) || (element.GetNode2() == temp))
                    return element;
            return null;
        }
    }
}
