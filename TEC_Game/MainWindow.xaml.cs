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
using System.Windows.Navigation;
using System.Windows.Shapes;
using tec;

namespace TEC_Game
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ExitButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Level1ChooseButton_Click(object sender, RoutedEventArgs e)
        {
            GameWindow gameWindow = new GameWindow();
            gameWindow.Owner = this;
            this.Hide();
            GameController controller = new GameController(new Player(), new Scheme());
            string dir = Environment.CurrentDirectory.Replace(@"bin\Debug", "");
            controller.InitializeScheme(dir + "Level1.txt");
            gameWindow.Show();
        }

        private void StatisticsButton_Click(object sender, RoutedEventArgs e)
        {
            StatisticsWindows statWindow = new StatisticsWindows();
            statWindow.Owner = this;
            this.Hide();
            statWindow.Show();
        }
    }
}
