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
            for (int i = 0; i < 40; i++)
            {
                gameWindow.GameGrid.RowDefinitions.Add(new RowDefinition());
            }
            for (int i = 0; i < 57; i++)
            {
                gameWindow.GameGrid.ColumnDefinitions.Add(new ColumnDefinition());
            }

            ContentControl control = new ContentControl
            {
                Template = gameWindow.FindResource("UndefinedTemplate") as ControlTemplate,
                Background = new SolidColorBrush(Color.FromRgb(225, 226, 225))
            };

            control.SetValue(Grid.RowSpanProperty, 40);
            control.SetValue(Grid.ColumnSpanProperty, 57);
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



        private void Dfs(BaseElement v)
        {
            var elements = new List<BaseElement>();
            elements.AddRange(v.GetNode1().GetConnectedElements());
            elements.AddRange(v.GetNode2().GetConnectedElements());

            foreach (var element in elements)
            {
                var a = element.GetNode1();
                var b = element.GetNode2();
                if (!AllElementsSet.Contains(element))
                {
                    AllElementsSet.Add(element);
                    Dfs(element);
                }
            }
        }

        private void RecalcElementsList()
        {
            AllElementsSet.Clear();
            var root = scheme.getRoot();
            foreach (var v in root.GetConnectedElements())
            {
                Dfs(v);
            }
        }

        public void OnSimplifyClicked(object sender, RoutedEventArgs e)
        {
            RecalcElementsList();
            foreach (var a in AllElementsSet)
            {
                foreach (var b in AllElementsSet)
                {
                    if (a == b) continue;

                    if ((a.GetNode1().GetId() == b.GetNode1().GetId() && a.GetNode2().GetId() == b.GetNode2().GetId()) ||
                        (a.GetNode1().GetId() == b.GetNode2().GetId() && a.GetNode2().GetId() == b.GetNode1().GetId()))
                    {
                        if (a is Conductor && (b is Nullator || b is Norator))
                        {
                            /**
                             * Если норатор/нуллатор и проводимость(она же conductor) соединены друг с другом в двух узлах
                             * */
                            scheme.RemoveElement(a, gameWindow.GameGrid);
                            gameWindow.GameGrid.Children.Remove(a.GetImage());
                        }
                        if (b is Conductor && (a is Nullator || a is Norator))
                        {
                            /**
                              * Если норатор/нуллатор и проводимость(она же conductor) соединены друг с другом в двух узлах
                              * */
                            scheme.RemoveElement(b, gameWindow.GameGrid);
                            gameWindow.GameGrid.Children.Remove(b.GetImage());
                        }
                    }
                }
            }

            foreach (var a in AllElementsSet)
            {
                foreach (var b in AllElementsSet)
                {
                    if (a == b) continue;

                    Node common = null;

                    if (a.GetNode1().GetId() == b.GetNode1().GetId()) common = a.GetNode1();
                    if (a.GetNode2().GetId() == b.GetNode2().GetId()) common = a.GetNode2();
                    if (a.GetNode1().GetId() == b.GetNode2().GetId()) common = a.GetNode1();

                    int count = 0;
                    foreach (var k in AllElementsSet)
                    {
                        if (k.GetNode1() == common || k.GetNode2() == common)
                            count++;
                    }
                    if (count != 2)
                        continue;

                    if (a is Resistor && (b is Nullator || b is Norator))
                    {
                        /**
                         *  Если норатор/нуллатор с резистором соединены одним узлом и при этом к этому узлу больше никто не подключен
                         * */
                        scheme.RemoveElement(a, gameWindow.GameGrid);
                        gameWindow.GameGrid.Children.Remove(a.GetImage());
                    }
                    if (b is Resistor && (a is Nullator || a is Norator))
                    {
                        /**
                          *  Если норатор/нуллатор с резистором соединены одним узлом и при этом к этому узлу больше никто не подключе
                          * */
                        scheme.RemoveElement(b, gameWindow.GameGrid);
                        gameWindow.GameGrid.Children.Remove(b.GetImage());
                    }
                }
            }

            scheme.RemoveNullor(gameWindow.GameGrid);
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
            RecalcElementsList();
            if (AllElementsSet.OfType<Nullator>().Count() != 0 && AllElementsSet.OfType<Norator>().Count() != 0)
            {
                if (!gameWindow.simplifyButton.IsEnabled)
                    opacityAnimation(gameWindow.simplifyButton, GetFadeInAnimation());
                gameWindow.simplifyButton.IsEnabled = true;
            }
            else
            {
                if (gameWindow.simplifyButton.IsEnabled)
                    opacityAnimation(gameWindow.simplifyButton, GetFadeInAnimation());
                gameWindow.simplifyButton.IsEnabled = false;
            }
        }

        public void OnNodeClick(object sender, RoutedEventArgs e)
        {
            Node node = sender as Node;
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
            schemeController.FindPlaceAndCreateNullor(player.GetNodeChosen1(), player.GetNodeChosen2(), "No");

            player.GetNodeChosen1().Background = Brushes.Black;
            player.GetNodeChosen2().Background = Brushes.Black;
            player.RemoveNode(player.GetNodeChosen1());
            player.RemoveNode(player.GetNodeChosen2());
            SetSimplifyStatus();
            e.Handled = true;
            DisableNullatorAndNoratorBtn();
        }

        public void OnNullatorButtonClick(object sender, RoutedEventArgs e)
        {
            schemeController.FindPlaceAndCreateNullor(player.GetNodeChosen1(), player.GetNodeChosen2(), "Nu");

            player.GetNodeChosen1().Background = Brushes.Black;
            player.GetNodeChosen2().Background = Brushes.Black;
            player.RemoveNode(player.GetNodeChosen1());
            player.RemoveNode(player.GetNodeChosen2());
            SetSimplifyStatus();
            e.Handled = true;
            DisableNullatorAndNoratorBtn();
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
