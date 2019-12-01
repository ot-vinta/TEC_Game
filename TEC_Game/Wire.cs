using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Data;
using System.Windows;
using System.ComponentModel;

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
            image.Stretch = Stretch.Fill;
            image.Source = new BitmapImage(new Uri(dir + @"Images\Wire.png"));
        }


        public void AddConnectedObject(object obj)
        {
            connectedObjects.Add(obj);
        }

        public void RemoveConnectedObject(object obj)
        {
            connectedObjects.Remove(obj);
        }

        public int ConnectedObjectsCount()
        {
            return connectedObjects.Count;
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
