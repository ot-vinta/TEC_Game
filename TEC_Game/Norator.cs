﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace tec
{
    class Norator : NullorElement
    {
        public Norator(Node node1, Node node2, int id) : base(node1, node2, id)
        {
            string dir = Environment.CurrentDirectory.Replace(@"bin\Debug", "");
            image = new Image();
            image.StretchDirection = StretchDirection.Both;
            image.Stretch = Stretch.Fill;
            image.Source = new BitmapImage(new Uri(dir + @"Images\Norator.png"));
            image.HorizontalAlignment = HorizontalAlignment.Stretch;
            image.VerticalAlignment = VerticalAlignment.Stretch;
        }

        public override void Destroy()
        {
            try
            {
                image.Source = null;
                image = null;
            }
            catch (NullReferenceException) { }
        }

        public override void ChangeImageDirectionToLand()
        {
            string dir = Environment.CurrentDirectory.Replace(@"bin\Debug", "");
            image.Source = new BitmapImage(new Uri(dir + @"Images\NoratorLand.png"));
        }
    }
}
