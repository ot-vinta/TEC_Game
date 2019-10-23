using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TEC_Game
{
    class Wire
    {
        private Image image;
        private int id, row, column;

        public Wire(int id, int row, int column)
        {
            this.id = id;
            this.row = row;
            this.column = column;

            string dir = Environment.CurrentDirectory.Replace(@"bin\Debug", "");
            image = new Image();
            image.StretchDirection = StretchDirection.Both;
            image.Source = new BitmapImage(new Uri(dir + @"Images\Wire.png"));
        }

        public void ChangeImageDirectionToLand()
        {
            string dir = Environment.CurrentDirectory.Replace(@"bin\Debug", "");
            image.Source = new BitmapImage(new Uri(dir + @"Images\WireLand.png"));
        }

        public Image GetImage()
        {
            return image;
        }

        public int GetId()
        {
            return id;
        }

        public int GetRow()
        {
            return row;
        }

        public int GetColumn()
        {
            return column;
        }

        public void Destroy()
        {
            //TO DO
        }
    }
}
