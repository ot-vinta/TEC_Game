using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
                    int Y_SIZE = Int32.Parse(GetSubString(ref line, line.IndexOf(' ')));

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

                        //Добавляем в схему узлы
                        Node node1 = new Node(new Button(), node1Id, node1X, node1Y);
                        Node node2 = new Node(new Button(), node2Id, node2X, node2Y);
                        scheme.AddNode(node1);
                        scheme.AddNode(node2);

                        //Кнопкам узлов добавляем номер
                        node1.GetButton().Content = node1Id.ToString();
                        node2.GetButton().Content = node2Id.ToString();
                        
                        //Добавляем элемент в зависимости от типа
                        if (type == "r")
                        {
                            var resistor = new Resistor(node1, node2, scheme.GetElementsSize());
                            scheme.AddElement(resistor);
                        }
                        else
                        {
                            var conductor = new Conductor(node1, node2, scheme.GetElementsSize());
                            scheme.AddElement(conductor);
                        }

                        //Находим объект игрового окна для добавления элементов в него
                        Window gameWindow;
                        foreach (Window window in Application.Current.Windows)
                            if (window is GameWindow) gameWindow = window;

                        //формат sh указывает на то, что между двумя нодами подключен только один элемент
                        if (format == "sh")
                        {


                            
                        }
                        //Этот формат показывает, что между 2 нодами есть несколько элементов
                        else if (format == "ff")
                        {
                            //Print it on screen
                        }
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
            line = line.Substring(len + 1, line.Length - len);
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
