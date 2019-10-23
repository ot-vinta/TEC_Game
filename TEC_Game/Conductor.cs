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
            image.StretchDirection = StretchDirection.Both;
            //image.RenderTransformOrigin = new Point(0, 0.5);
            image.Source = new BitmapImage(new Uri(dir + @"Images\Conductor.png"));
            image.HorizontalAlignment = HorizontalAlignment.Stretch;
            image.VerticalAlignment = VerticalAlignment.Stretch;
        }

        public override void ChangeImageDirectionToLand()
        {
            string dir = Environment.CurrentDirectory.Replace(@"bin\Debug", "");
            image.Source = new BitmapImage(new Uri(dir + @"Images\ConductorLand.png"));
        }

        public override void Destroy()
        {
            //TO DO
        }
    }
}
