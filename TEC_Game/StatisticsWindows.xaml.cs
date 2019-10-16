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

namespace TEC_Game
{
    /// <summary>
    /// Логика взаимодействия для StatisticsWindows.xaml
    /// </summary>
    public partial class StatisticsWindows : Window
    {
        public StatisticsWindows()
        {
            InitializeComponent();
            SetNumFinished(NumFinished);
        }
        public void SetNumFinished(Label label)
        {
            int content = 23;
            label.Content = content.ToString();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Owner.Show();
            this.Close();
        }
    }
}
