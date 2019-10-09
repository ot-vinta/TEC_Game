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
using tec;

namespace TEC_Game
{
    /// <summary>
    /// Логика взаимодействия для ChooseLevelWindow.xaml
    /// </summary>
    public partial class ChooseLevelWindow : Window
    {
        public ChooseLevelWindow()
        {
            InitializeComponent();
        }

        private void GoBackButton_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void Level1Button_Click(object sender, RoutedEventArgs e)
        {
            GameWindow gameWindow = new GameWindow();
            GameController controller = new GameController(new Scheme(), new Player());
            string dir = Environment.CurrentDirectory.Replace(@"bin\Debug", "");
            controller.InitializeScheme(dir + "Level1.txt");
            gameWindow.Show();
        }
    }
}
