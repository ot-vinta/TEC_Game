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
using tec;

namespace TEC_Game
{
    class Wire
    {
        private Image image;
        private int id, row, column;
        private object obj1, obj2;

        public Wire(int id, int row, int column)
        {
            this.id = id;
            this.row = row;
            this.column = column;
            string dir = Environment.CurrentDirectory.Replace(@"bin\Debug", "");
            obj1 = null;
            obj2 = null;
            image = new Image();
            image.StretchDirection = StretchDirection.Both;
            image.Stretch = Stretch.Fill;
            image.Source = new BitmapImage(new Uri(dir + @"Images\Wire.png"));
        }

        public void AddConnectedObject(object obj)
        {
            if (obj1 == null) obj1 = obj;
            else obj2 = obj;
        }

        public void RemoveObject(object obj)
        {
            if (obj1 == obj) obj1 = null;
            else if (obj2 == obj) obj2 = null;
        }

        public int GetObjectsCount()
        {
            if (obj1 != null && obj2 != null) return 2;
            if (obj1 == null && obj2 == null) return 0;
            return 1;
        }

        public object GetObject1()
        {
            return obj1;
        }

        public object GetObject2()
        {
            return obj2;
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
            try
            {
                image.Source = null;
                image = null;
            }
            catch (NullReferenceException) { }
        }
    }
}
