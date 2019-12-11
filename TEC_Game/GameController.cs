using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using TEC_Game;

namespace tec
{
    class GameController
    {
        public GameWindow gameWindow;
        public Scheme scheme;
        public Player player;
        private SchemeController schemeController;

        HashSet<BaseElement> AllElementsSet = new HashSet<BaseElement>();

        public GameController(Player player, Scheme scheme)
        {
            this.player = player;
            this.scheme = scheme;
            schemeController = new SchemeController(this);

            //Находим объект игрового окна для добавления элементов в него
            gameWindow = null;
            foreach (Window window in Application.Current.Windows)
                if (window is GameWindow) gameWindow = window as GameWindow;

            MakeGrid();

            //Обработчики для кнопок, добавляющих нуллор
            gameWindow.addNoratorButton.Click += new RoutedEventHandler(OnNoratorButtonClick);
            gameWindow.addNullatorButton.Click += new RoutedEventHandler(OnNullatorButtonClick);

            gameWindow.simplifyButton.Click += new RoutedEventHandler(OnSimplifyClicked);
        }

        private void MakeGrid()
        {
            for (int i = 0; i < 50; i++)
            {
                gameWindow.GameGrid.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < 70; i++)
            {
                gameWindow.GameGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            ContentControl control = new ContentControl
            {
                Template = gameWindow.FindResource("UndefinedTemplate") as ControlTemplate,
                Background = new SolidColorBrush(Color.FromRgb(225, 226, 225))
            };

            control.SetValue(Grid.RowSpanProperty, 50);
            control.SetValue(Grid.ColumnSpanProperty, 70);
            control.SetValue(Panel.ZIndexProperty, 0);

            gameWindow.GameGrid.Children.Add(control);
        }

        public void InitializeScheme(String path)
        {
            try
            {
                using (StreamReader reader = new StreamReader(path))
                {
                    string line = reader.ReadLine();

                    while (line != "")
                    {
                        string type = schemeController.GetSubString(ref line, 2);
                        switch (type)
                        {
                            case "Nd":
                                schemeController.PlaceNode(ref line);
                                break;
                            case "Co":
                            case "Re":
                                schemeController.PlaceElement(ref line, type);
                                break;
                            case "Wi":
                                schemeController.PlaceWire(ref line);
                                break;
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

        private void Eliminate(HashSet<Node> usedNodes, HashSet<BaseElement> usedElements)
        {
            var deletedElements = 1;
            while (true)
            {
                var norator = scheme.FindNorator();
                var nullator = scheme.FindNullator();

                if (deletedElements != 0)
                {
                    deletedElements = 0;

                    foreach (var elem in usedNodes.SelectMany(node => node.GetConnectedElements()))
                    {
                        usedElements.Add(elem);
                    }

                    usedElements.Remove(norator);
                    usedElements.Remove(nullator);

                    HashSet<BaseElement> elementsToDelete = new HashSet<BaseElement>();

                    foreach (var element in usedElements)
                    {
                        var eNode1 = element.GetNode1();
                        var eNode2 = element.GetNode2();
                        var norNode1 = norator.GetNode1();
                        var norNode2 = norator.GetNode2();
                        var nullNode1 = nullator.GetNode1();
                        var nullNode2 = nullator.GetNode2();

                        switch (element)
                        {
                            case Conductor _ when (eNode1 == norNode1 && eNode2 == norNode2 || 
                                                   eNode1 == norNode2 && eNode2 == norNode1 || 
                                                   eNode1 == nullNode1 && eNode2 == nullNode2 || 
                                                   eNode1 == nullNode2 && eNode2 == nullNode1):
                                elementsToDelete.Add(element);
                                deletedElements++;
                                break;
                            case Resistor _:
                            {
                                if (eNode1 == norNode1 && eNode2 != norNode2 &&
                                    eNode1.GetConnectedElements().Count == 2 ||
                                    eNode1 == norNode2 && eNode2 != norNode1 &&
                                    eNode1.GetConnectedElements().Count == 2 ||
                                    eNode1 == nullNode1 && eNode2 != nullNode2 &&
                                    eNode1.GetConnectedElements().Count == 2 ||
                                    eNode1 == nullNode2 && eNode2 != nullNode1 &&
                                    eNode1.GetConnectedElements().Count == 2)
                                {
                                    elementsToDelete.Add(element);
                                    deletedElements++;
                                    usedNodes.Add(eNode2);
                                }
                                else if (eNode1 != norNode1 && eNode2 == norNode2 &&
                                         eNode2.GetConnectedElements().Count == 2 ||
                                         eNode1 != norNode2 && eNode2 == norNode1 &&
                                         eNode2.GetConnectedElements().Count == 2 ||
                                         eNode1 != nullNode1 && eNode2 == nullNode2 &&
                                         eNode2.GetConnectedElements().Count == 2 ||
                                         eNode1 != nullNode2 && eNode2 == nullNode1 &&
                                         eNode2.GetConnectedElements().Count == 2)
                                {
                                    elementsToDelete.Add(element);
                                    deletedElements++;
                                    usedNodes.Add(eNode1);
                                }

                                break;
                            }
                        }
                    }

                    foreach (var element in elementsToDelete) SayGoodBye(element, ref usedElements);

                    HashSet<Wire> wiresToDelete = new HashSet<Wire>();
                    foreach (var wire in scheme.GetWires())
                        if (wire != null)
                        {
                            if (wire.GetObjectsCount() <= 1)
                                wiresToDelete.Add(wire);
                        }

                    foreach (var wire in wiresToDelete)
                    {
                        DeleteWire(wire);
                    }
                    continue;
                }

                break;
            }
        }

        private void DeleteWire(Wire wire)
        {
            scheme.RemoveWire(wire);
            gameWindow.GameGrid.Children.Remove(wire.GetImage());
        }

        private void SayGoodBye(BaseElement element, ref HashSet<BaseElement> usedElements)
        {
            usedElements.Remove(element);
            scheme.RemoveElement(element, gameWindow.GameGrid);
            gameWindow.GameGrid.Children.Remove(element.GetImage());
        }

        public void OnSimplifyClicked(object sender, RoutedEventArgs e)
        {
            HashSet<BaseElement> usedElements = new HashSet<BaseElement>();
            HashSet<Node> usedNodes = new HashSet<Node>();

            var norator = scheme.FindNorator();
            var nullator = scheme.FindNullator();

            usedNodes.Add(norator.GetNode1());
            usedNodes.Add(norator.GetNode2());
            usedNodes.Add(nullator.GetNode1());
            usedNodes.Add(nullator.GetNode2());

            Eliminate(usedNodes, usedElements);

            Node nullNode1 = scheme.FindNullator().GetNode1();
            Node nullNode2 = scheme.FindNullator().GetNode2();
            Node norNode1 = scheme.FindNorator().GetNode1();
            Node norNode2 = scheme.FindNorator().GetNode2();

            schemeController.FindPlaceAndCreateNullor(nullNode1, nullNode2, "Nu");
            schemeController.FindPlaceAndCreateNullor(norNode1, norNode2, "No");

            scheme.RemoveNullor(gameWindow.GameGrid, norator, nullator);

            HashSet<Node> nodesToDelete = new HashSet<Node>();

            foreach (var node in scheme.GetNodes())
                if (node != null && node.GetConnectedElementsCount() == 0 && node.GetConnectedWires().Count == 0)
                {
                    nodesToDelete.Add(node);
                }

            foreach (var temp in nodesToDelete)
            {
                scheme.RemoveNode(temp);
                gameWindow.GameGrid.Children.Remove(temp);
            }

            CheckWires();

            var deletedWires = 1;
            while (deletedWires > 0)
            {
                deletedWires = 0;

                HashSet<Wire> wiresToDelete = new HashSet<Wire>();
                foreach (var wire in scheme.GetWires())
                    if (wire != null)
                    {
                        if (wire.GetObjectsCount() <= 1)
                        {
                            deletedWires++;
                            wiresToDelete.Add(wire);
                            if (wire.GetObject1() is Wire)
                            {
                                Wire temp = (Wire) wire.GetObject1();
                                temp.RemoveObject(wire);
                            }

                            if (wire.GetObject2() is Wire)
                            {
                                Wire temp = (Wire) wire.GetObject2();
                                temp.RemoveObject(wire);
                            }
                        }
                    }

                foreach (var wire in wiresToDelete)
                {
                    DeleteWire(wire);
                }
            }

            SetSimplifyStatus();
        }

        private void CheckWires()
        {
            foreach (var wire in scheme.GetWires())
                if (wire != null)
                {
                    if (wire.GetObject1() is Node)
                    {
                        Node temp1 = (Node) wire.GetObject1();
                        if (scheme.GetNodes()[temp1.GetId() - 1] == null)
                        {
                            wire.RemoveObject(temp1);
                        }
                    }

                    if (wire.GetObject2() is Node)
                    {
                        Node temp2 = (Node) wire.GetObject2();
                        if (scheme.GetNodes()[temp2.GetId() - 1] == null)
                        {
                            wire.RemoveObject(temp2);
                        }
                    }
                }
        }


        private void OnNodeLeave(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            button.Background = Brushes.Black;
        }

        private void OnNodeEnter(object sender, MouseEventArgs e)
        {
            Button button = sender as Button;
            button.Background = Brushes.Aqua;
        }

        public void SetSimplifyStatus()
        {
            if (gameWindow.simplifyButton.IsEnabled == false)
            {
                if (!gameWindow.simplifyButton.IsEnabled)
                    opacityAnimation(gameWindow.simplifyButton, GetFadeInAnimation());
                gameWindow.simplifyButton.IsEnabled = true;
            }
            else
            {
                if (gameWindow.simplifyButton.IsEnabled)
                    opacityAnimation(gameWindow.simplifyButton, GetFadeOutAnimation());
                gameWindow.simplifyButton.IsEnabled = false;
            }
        }

        public void OnNodeClick(object sender, RoutedEventArgs e)
        {
            Node node = sender as Node;
            HideAlarm();

            if ((node.Background == Brushes.Black) && (!player.NodesChosen()))
            {
                node.Background = Brushes.Blue;

                player.ChooseNode(node);

                if (player.NodesChosen())
                {
                    //Here we need to add animations to slides
                    gameWindow.addNoratorButton.IsEnabled = true;
                    gameWindow.addNoratorButton.Background = new SolidColorBrush(Color.FromRgb(255, 110, 64));
                    gameWindow.addNullatorButton.IsEnabled = true;
                    gameWindow.addNullatorButton.Background = new SolidColorBrush(Color.FromRgb(255, 110, 64));
                }
            }
            else if ((node.Background == Brushes.Black) && (player.NodesChosen()))
            {
                //Make text on screen
                //print alarm on it
            }
            else if (node.Background == Brushes.Blue)
            {
                node.Background = Brushes.Black;

                if (player.NodesChosen())
                {
                    //Here we need to add animations to slides
                    gameWindow.addNoratorButton.IsEnabled = false;
                    gameWindow.addNoratorButton.Background = new SolidColorBrush(Color.FromRgb(128, 128, 128));
                    gameWindow.addNullatorButton.IsEnabled = false;
                    gameWindow.addNullatorButton.Background = new SolidColorBrush(Color.FromRgb(128, 128, 128));
                }

                player.RemoveNode(node);
            }

            e.Handled = true;

            if (player.NodesChosen())
            {
                gameWindow.addNullatorButton.OpacityMask = null;
                gameWindow.addNoratorButton.OpacityMask = null;

                if (gameWindow.addNullatorButton.Opacity == 0)
                    opacityAnimation(gameWindow.addNullatorButton, GetFadeInAnimation());
                if (gameWindow.addNoratorButton.Opacity == 0)
                    opacityAnimation(gameWindow.addNoratorButton, GetFadeInAnimation());

                gameWindow.addNullatorButton.IsEnabled = true;
                gameWindow.addNoratorButton.IsEnabled = true;
            }
            else
            {
                if (gameWindow.addNullatorButton.Opacity == 1)
                    opacityAnimation(gameWindow.addNullatorButton, GetFadeOutAnimation());
                if (gameWindow.addNoratorButton.Opacity == 1)
                    opacityAnimation(gameWindow.addNoratorButton, GetFadeOutAnimation());

                gameWindow.addNullatorButton.IsEnabled = false;
                gameWindow.addNoratorButton.IsEnabled = false;
            }
        }

        DoubleAnimation GetFadeAnimation(Double fromValue, Double toValue)
        {
            var anim = new DoubleAnimation();
            anim.From = fromValue;
            anim.FillBehavior = FillBehavior.HoldEnd;
            anim.To = toValue;
            anim.Duration = TimeSpan.FromSeconds(0.6);
            return anim;
        }

        DoubleAnimation GetFadeInAnimation()
        {
            return GetFadeAnimation(0, 1);
        }

        DoubleAnimation GetFadeOutAnimation()
        {
            return GetFadeAnimation(1, 0);
        }

        void opacityAnimation(DependencyObject view, DoubleAnimation animation)
        {

            Storyboard.SetTarget(animation, view);
            var f = animation.TargetPropertyType.Name;
            Storyboard.SetTargetProperty(animation, new PropertyPath("(Opacity)"));

            var storyboard = new Storyboard();
            storyboard.Children = new TimelineCollection { animation };
            storyboard.Begin();
        }

        public void OnNoratorButtonClick(object sender, RoutedEventArgs e)
        {
            if (scheme.FindNorator() == null)
            {
                HideAlarm();
                schemeController.FindPlaceAndCreateNullor(player.GetNodeChosen1(), player.GetNodeChosen2(), "No");

                player.GetNodeChosen1().Background = Brushes.Black;
                player.GetNodeChosen2().Background = Brushes.Black;
                player.RemoveNode(player.GetNodeChosen1());
                player.RemoveNode(player.GetNodeChosen2());
                if (scheme.FindNullator() != null && scheme.FindNorator() != null)
                    SetSimplifyStatus();
                e.Handled = true;
                DisableNullatorAndNoratorBtn();
            }
            else Alarm("Норатор уже есть!");
        }

        public void OnNullatorButtonClick(object sender, RoutedEventArgs e)
        {
            if (scheme.FindNullator() == null)
            {
                HideAlarm();
                schemeController.FindPlaceAndCreateNullor(player.GetNodeChosen1(), player.GetNodeChosen2(), "Nu");

                player.GetNodeChosen1().Background = Brushes.Black;
                player.GetNodeChosen2().Background = Brushes.Black;
                player.RemoveNode(player.GetNodeChosen1());
                player.RemoveNode(player.GetNodeChosen2());
                if (scheme.FindNullator() != null && scheme.FindNorator() != null)
                    SetSimplifyStatus();
                e.Handled = true;
                DisableNullatorAndNoratorBtn();
            }
            else Alarm("Нуллатор уже есть!");
        }

        private void Alarm(string text)
        {
            gameWindow.alarmText.Visibility = Visibility.Visible;
            gameWindow.alarmText.Text = text;
        }

        private void HideAlarm()
        {
            gameWindow.alarmText.Visibility = Visibility.Hidden;
        }

        void DisableNullatorAndNoratorBtn()
        {
            SolidColorBrush grayBrush = new SolidColorBrush(Colors.Gray);
            grayBrush.Opacity = 0;
            gameWindow.addNullatorButton.OpacityMask = grayBrush;
            gameWindow.addNoratorButton.OpacityMask = grayBrush;

            gameWindow.addNullatorButton.Opacity = 0.0;
            gameWindow.addNoratorButton.Opacity = 0.0;
            gameWindow.addNullatorButton.IsEnabled = false;
            gameWindow.addNoratorButton.IsEnabled = false;
        }

        public void ChangeNullorDirection(NullorElement element)
        {
            if (element.GetDirection() == "right")
                element.SetDirection("left");
            else
                element.SetDirection("right");
        }
    }
}
