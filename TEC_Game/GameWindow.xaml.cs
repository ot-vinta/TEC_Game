using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Media.Animation;

namespace TEC_Game
{
    /// <summary>
    /// Логика взаимодействия для GameWindow.xaml
    /// </summary>
    public partial class GameWindow : Window
    {
        public GameWindow()
        {
            InitializeComponent();

        }
        private void Window_Loaded(object sender,RoutedEventArgs e)
        {
            //Rectangle rectangle = new Rectangle();
            //rectangle.Width = 300;
            //rectangle.Height = 70;
            //rectangle.Fill = Brushes.Blue;
            //Canvas.SetLeft(rectangle, 900);
            //Canvas.SetTop(rectangle, 70);
            //elem1.Children.Insert(0, rectangle);
        }

        private void ToMainMenuButton_Click(object sender, RoutedEventArgs e)
        {

        }
        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }
    }
}
