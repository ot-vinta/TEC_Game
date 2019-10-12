using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace tec
{
    class Conductor : BaseElement
    {
        public Conductor(Node node1, Node node2, int id) : base(node1, node2, id)
        {
            string dir = Environment.CurrentDirectory.Replace(@"bin\Debug", "");
            image = new Image();
            image.Source = new BitmapImage(new Uri(dir + @"Images\Conductor.png"));
            image.Width = 50;
            image.Height = 200;
            image.HorizontalAlignment = HorizontalAlignment.Left;
            image.VerticalAlignment = VerticalAlignment.Top;
        }

        public override void Destroy()
        {
            //TO DO
        }
    }
}
