using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Controls;
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

            Node node = new Node(new Button(), id, row, column);

            gameController.scheme.AddNode(node); //Добавление узла в схему

            node.GetButton().Template = gameController.gameWindow.FindResource("NodeTemplate") as ControlTemplate; //Задания шаблона для узла

            node.GetButton().Click += new RoutedEventHandler(gameController.OnNodeClick); //Добавление обработчика нажатия

            node.GetButton().Content = id.ToString(); //Задание текста на узле

            node.GetButton().SetValue(Grid.RowProperty, row); //Задание положения узла в grid
            node.GetButton().SetValue(Grid.ColumnProperty, column);
            node.GetButton().SetValue(Panel.ZIndexProperty, 2);

            gameController.gameWindow.GameGrid.Children.Add(node.GetButton()); //Добавление узла в grid
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

            if (type == "Re")
            {
                element = new Resistor(node1, node2, id);
            }
            else
            {
                element = new Conductor(node1, node2, id);
            }

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

            int row = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));
            int column = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));

            string direction = GetSubString(ref line, 1);

            Wire wire = new Wire(id, row, column);

            wire.GetImage().SetValue(Grid.RowProperty, row);
            wire.GetImage().SetValue(Grid.ColumnProperty, column);
            wire.GetImage().SetValue(Panel.ZIndexProperty, 1);

            if (direction == "R")
            {
                wire.ChangeImageDirectionToLand();
                wire.GetImage().SetValue(Grid.ColumnSpanProperty, 4);
            }
            else
            {
                wire.GetImage().SetValue(Grid.RowSpanProperty, 4);
            }

            gameController.gameWindow.GameGrid.Children.Add(wire.GetImage());
        }

        public string GetSubString(ref string line, int len)
        {
            string ans = line.Substring(0, len);
            if (line.Length <= len + 1)
                line = "";
            else
                line = line.Substring(len + 1);
            return ans;
        }

        public void PlaceNullor()
        {
            Node node1 = gameController.scheme.GetNode(gameController.player.GetNodeChosen1());
            Node node2 = gameController.scheme.GetNode(gameController.player.GetNodeChosen2());

            //Получаем путь между узлами
            List<Node> wayBetweenNodes = FindWay(node1, node2);

            if ((node1.GetX() == node2.GetX()) && (node1.GetY() != node2.GetY()))
            {
                //Если элемент будет расположен горизонтально
            }
            else if ((node1.GetX() != node2.GetX()) && (node1.GetY() == node2.GetY()))
            {
                //Если элемент будет расположен вертикально
            }
            else
            {
                //Если есть выбор
            }
        }

        private List<Node> FindWay(Node node1, Node node2)
        {
            List<Node> ans = new List<Node>();
            List<Node> markedNodes = new List<Node> {node1};

            Queue<Way> queue = new Queue<Way>();

            //Заменить все на один метод, который внизу будет
            List<BaseElement> elements = node1.GetConnectedElements();
            foreach (var element in elements)
            {
                Node elementNode1 = element.GetNode1();
                Node elementNode2 = element.GetNode2();

                queue.Enqueue(elementNode1 == node1
                    ? new Way(new List<Node> {node1, elementNode2})
                    : new Way(new List<Node> {node1, elementNode1}));
            }

            while (queue.Peek().GetFirstNode() != node2)
            {
                Node firstNode = queue.Dequeue().GetFirstNode();
                elements = node1.GetConnectedElements();

                //Заменить все на один метод, который внизу будет
            }

            //Получить путь между узлами

            return ans;
        }

        //Сделать метод для добавления новых путей в очередь

        private class Way
        {
            private List<Node> nodes;

            internal Way(List<Node> nodes)
            {
                this.nodes = nodes;
            }

            internal Node GetFirstNode()
            {
                return nodes[nodes.Count - 1];
            }
        }
    }
}
