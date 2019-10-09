using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TEC_Game;

namespace tec
{
    class GameController
    {
        private Scheme scheme;
        private Player player;

        public GameController(Scheme scheme, Player player)
        {
            this.scheme = scheme;
            this.player = player;
        }

        public void InitializeScheme(String path)
        {
            const int X_CENTER = 450;
            const int Y_CENTER = 390;
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string line = reader.ReadLine();
                    int X_SIZE = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));
                    int Y_SIZE = Int32.Parse(GetSubString(ref line, line.Length));

                    line = reader.ReadLine();
                    while (line != "")
                    {
                        //Определяем формат данных
                        string format = GetSubString(ref line, 2);

                        //Получаем данные для узлов
                        int node1Id = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));
                        int node1X = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));
                        int node1Y = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));

                        int node2Id = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));
                        int node2X = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));
                        int node2Y = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));

                        //Определеям тип элемента(r - резистор, p - проводимость
                        string type = GetSubString(ref line, 1);

                        Node node1 = scheme.GetNode(node1Id);
                        Node node2 = scheme.GetNode(node2Id);

                        Boolean isNode1NotNull = false;
                        Boolean isNode2NotNull = false;

                        //Если узлов еще не в схеме, то добавляем их
                        if (node1 == null)
                        {
                            isNode1NotNull = true;
                            node1 = new Node(new Button(), node1Id, node1X, node1Y);
                            scheme.AddNode(node1);
                        }

                        if (node2 == null)
                        {
                            isNode2NotNull = true;
                            node2 = new Node(new Button(), node2Id, node2X, node2Y);
                            scheme.AddNode(node2);
                        }

                        BaseElement element = null;

                        //Добавляем элемент в зависимости от типа
                        if (type == "r")
                        {
                            element = new Resistor(node1, node2, scheme.GetElementsSize());
                            scheme.AddElement(element);
                        }
                        else
                        {
                            element = new Conductor(node1, node2, scheme.GetElementsSize());
                            scheme.AddElement(element);
                        }

                        //Находим объект игрового окна для добавления элементов в него
                        GameWindow gameWindow = null;
                        foreach (Window window in Application.Current.Windows)
                            if (window is GameWindow) gameWindow = window as GameWindow;

                        //Добавляем шаблон кнопкам
                        node1.GetButton().Template = gameWindow.FindResource("NodeTemplate") as ControlTemplate;
                        node2.GetButton().Template = gameWindow.FindResource("NodeTemplate") as ControlTemplate;

                        //Кнопкам узлов добавляем номер
                        node1.GetButton().Content = node1Id.ToString(); //НЕ РАБОТАЕТ
                        node2.GetButton().Content = node2Id.ToString();

                        //формат sh указывает на то, что между двумя нодами подключен только один элемент
                        if (format == "sh")
                        {
                            double dx = -12, dy = 13;

                            if (node1X != node2X)
                            {
                                RotateTransform rotate = new RotateTransform(90);
                                element.GetImage().RenderTransform = rotate;
                                dx = 212;
                                dy = -13;
                            }

                            double x = X_CENTER - 200 * ((X_SIZE - 1) / 2 - node1X);
                            double y = Y_CENTER - 200 * ((Y_SIZE - 1) / 2 - node1Y);

                            element.GetImage().Margin = new Thickness(x + dx, y + dy, 0, 0);
                            gameWindow.GameGrid.Children.Add(element.GetImage());

                            if (isNode1NotNull)
                            {
                                double x1 = X_CENTER - 200 * ((X_SIZE - 1) / 2 - node1X);
                                double y1 = Y_CENTER - 200 * ((Y_SIZE - 1) / 2 - node1Y);

                                node1.GetButton().Margin = new Thickness(x1, y1, 0, 0);
                                gameWindow.GameGrid.Children.Add(node1.GetButton());
                            }

                            if (isNode2NotNull)
                            {
                                double x2 = X_CENTER - 200 * ((X_SIZE - 1) / 2 - node2X);
                                double y2 = Y_CENTER - 200 * ((Y_SIZE - 1) / 2 - node2Y);

                                node2.GetButton().Margin = new Thickness(x2, y2, 0, 0);
                                gameWindow.GameGrid.Children.Add(node2.GetButton());
                            }
                        }
                        //Этот формат показывает, что между 2 нодами есть несколько элементов
                        else if (format == "ff")
                        {
                            double dx = -12, dy = 13;
                            double nodeDx = 0, nodeDy = 0;
                            double x = 0, y = 0;
                            int n = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));
                            string direction = GetSubString(ref line, 1);
                            string isNodeNeeded = GetSubString(ref line, 1);

                            switch (direction)
                            {
                                case "D":
                                    dy += 75 * n;
                                    nodeDy = 75 * n;
                                    break;
                                case "U":
                                    dy -= 75 * n;
                                    nodeDy = -75 * n;
                                    break;
                                case "L":
                                    dx -= 75 * n;
                                    nodeDx = -75 * n;
                                    break;
                                case "R":
                                    dx += 75 * n;
                                    nodeDx = 75 * n;
                                    break;
                            }

                            if ((isNodeNeeded == "y") && (n != 0))
                            {
                                double x1 = X_CENTER - 200 * ((X_SIZE - 1) / 2 - node1X) + 23;
                                double y1 = Y_CENTER - 200 * ((Y_SIZE - 1) / 2 - node1Y) - 75;
                                double x2 = X_CENTER - 200 * ((X_SIZE - 1) / 2 - node1X) + 23;
                                double y2 = Y_CENTER - 200 * ((Y_SIZE - 1) / 2 - node1Y) - 75;

                                string dir = Environment.CurrentDirectory.Replace(@"bin\Debug", "");
                                Image wire1 = new Image();
                                wire1.Source = new BitmapImage(new Uri(dir + @"Images\Wire.png"));
                                wire1.Width = 3;
                                wire1.Height = 75;
                                wire1.HorizontalAlignment = HorizontalAlignment.Left;
                                wire1.VerticalAlignment = VerticalAlignment.Top;

                                Image wire2 = new Image();
                                wire2.Source = new BitmapImage(new Uri(dir + @"Images\Wire.png"));
                                wire2.Width = 3;
                                wire2.Height = 75;
                                wire2.HorizontalAlignment = HorizontalAlignment.Left;
                                wire2.VerticalAlignment = VerticalAlignment.Top;

                                if (node1X == node2X)
                                {
                                    RotateTransform rotate = new RotateTransform(90);
                                    wire1.RenderTransform = rotate;
                                    wire2.RenderTransform = rotate;
                                    y2 += 200;
                                }
                                else
                                {
                                    x2 += 200;
                                }

                                wire1.Margin = new Thickness(x1 + dx, y1 + dy, 0, 0);
                                gameWindow.GameGrid.Children.Add(wire1);
                                wire2.Margin = new Thickness(x2 + dx, y2 + dy, 0, 0);
                                gameWindow.GameGrid.Children.Add(wire2);
                            }

                            if (node1X != node2X)
                            {
                                RotateTransform rotate = new RotateTransform(90);
                                element.GetImage().RenderTransform = rotate;
                                dx += 225;
                                dy -= 26;
                            }

                            x = X_CENTER - 200 * ((X_SIZE - 1) / 2 - node1X);
                            y = Y_CENTER - 200 * ((Y_SIZE - 1) / 2 - node1Y);

                            element.GetImage().Margin = new Thickness(x + dx, y + dy, 0, 0);
                            gameWindow.GameGrid.Children.Add(element.GetImage());

                            if (isNode1NotNull)
                            {
                                double x1 = X_CENTER - 200 * ((X_SIZE - 1) / 2 - node1X) + nodeDx;
                                double y1 = Y_CENTER - 200 * ((Y_SIZE - 1) / 2 - node1Y) + nodeDy;

                                node1.GetButton().Margin = new Thickness(x1, y1, 0, 0);
                                gameWindow.GameGrid.Children.Add(node1.GetButton());
                            }

                            if (isNode2NotNull)
                            {
                                double x2 = X_CENTER - 200 * ((X_SIZE - 1) / 2 - node2X) + nodeDx;
                                double y2 = Y_CENTER - 200 * ((Y_SIZE - 1) / 2 - node2Y) + nodeDy;

                                node2.GetButton().Margin = new Thickness(x2, y2, 0, 0);
                                gameWindow.GameGrid.Children.Add(node2.GetButton());
                            }
                        }

                        line = reader.ReadLine();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private string GetSubString(ref string line, int len)
        {
            string ans = line.Substring(0, len);
            if (line.Length <= len + 1) 
                line = "";
            else 
                line = line.Substring(len + 1);
            return ans;
        }

        public void StartElimination()
        {
            if (!scheme.SchemeIsConnected())
            {
                //Print warning
                scheme.RemoveElement(scheme.FindNullator());
                scheme.RemoveElement(scheme.FindNorator());
            }
            else
            {
                //Добавляем в очередь ноды, соединенные с нуллором
                Queue<Node> queue = new Queue<Node>();
                queue.Enqueue(scheme.FindNullator().GetNode1());
                queue.Enqueue(scheme.FindNullator().GetNode2());
                queue.Enqueue(scheme.FindNorator().GetNode1());
                queue.Enqueue(scheme.FindNorator().GetNode2());

                //Из схемы можно убрать нуллор, он больше не нужен
                scheme.RemoveElement(scheme.FindNorator());
                scheme.RemoveElement(scheme.FindNullator());
                while (queue.Count > 0)
                {
                    Node node = queue.Dequeue();
                    Resistor aloneElement = node.GetResistor();

                    //Если подключен только один резистор, его можно убрать
                    if ((scheme.GetNodeConnectionsCount(node) == 1) && (aloneElement != null))
                    {
                        if (node.GetId() == aloneElement.GetNode1().GetId())
                            queue.Enqueue(aloneElement.GetNode2());
                        else
                            queue.Enqueue(aloneElement.GetNode1());
                        scheme.RemoveElement(aloneElement);
                    }
                    //В случае, когда к узлу подключено много элементов, есть смысл убирать только проводимости
                    else if ((scheme.GetNodeConnectionsCount(node) > 1) && (node.GetConductor() != null))
                    {
                        Conductor conductor = node.GetConductor();
                        while (conductor != null)
                        {
                            scheme.RemoveElement(conductor);
                            conductor = node.GetConductor();
                        }
                    }
                }
            }
        }

        public void ChangeDirection(NullorElement element)
        {
            if (element.GetDirection() == "right") 
                element.SetDirection("left");
            else
                element.SetDirection("right");
        }

        public void AddNullator(Node node1, Node node2)
        {
            Nullator nullator = new Nullator(node1, node2, scheme.GetElementsSize());
            scheme.AddElement(nullator);
        }

        public void AddNorator(Node node1, Node node2)
        {
            Norator norator = new Norator(node1, node2, scheme.GetElementsSize());
            scheme.AddElement(norator);
        }

        public void DestroyElement(BaseElement element)
        {
            scheme.RemoveElement(element);
        }
    }
}
