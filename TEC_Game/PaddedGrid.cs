using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace TEC_Game
{
    class PaddedGrid : Grid
    {
        // Класс создан для возможности привязать провод к сетке
        private static readonly DependencyProperty PaddingProperty =
        DependencyProperty.Register("Padding",
            typeof(Thickness), typeof(PaddedGrid),
            new UIPropertyMetadata(new Thickness(0.0),
            new PropertyChangedCallback(OnPaddingChanged)));
        public PaddedGrid()
        {
            //  Add a loded event handler.
            Loaded += new RoutedEventHandler(PaddedGrid_Loaded);
            //debug
            string Path = Environment.CurrentDirectory.Replace(@"bin\Debug", "") + "\\log.txt";
            using (StreamWriter writer = File.AppendText(Path))
                writer.WriteLine("Constructor is called");
            //end debug
        }
        void PaddedGrid_Loaded(object sender, RoutedEventArgs e)
        {
            {
                //debug
                string Path = Environment.CurrentDirectory.Replace(@"bin\Debug", "") + "\\log.txt";
                using (StreamWriter writer = File.AppendText(Path))
                    writer.WriteLine("PaddedGrid_Loaded is called");
                //end debug
            }
            //  Get the number of children.
            int childCount = VisualTreeHelper.GetChildrenCount(this);

            //  Go through the children.
            for (int i = 0; i < childCount; i++)
            {
                //  Get the child.
                DependencyObject child = VisualTreeHelper.GetChild(this, i);

                //  Try and get the margin property.
                //DependencyProperty halignProperty = GetHorizontalAlignment(child);
                //DependencyProperty valignProperty = GetVerticalAlignment(child);
                DependencyProperty marginProperty = GetMarginProperty(child);


                //  If we have a margin property, bind it to the padding.
                /*
                if (halignProperty != null)
                {
                    //  Create the binding.
                    Binding binding = new Binding
                    {
                        Source = this,
                        Path = new PropertyPath("HorizontalAlignment")
                    };

                    //  Bind the child's margin to the grid's padding.
                    BindingOperations.SetBinding(child, HorizontalAlignmentProperty, binding);
                }
                else
                {
                    //debug
                    string Path = Environment.CurrentDirectory.Replace(@"bin\Debug", "") + "\\log.txt";
                    StreamWriter file = new StreamWriter(Path);
                    file.WriteLine("No horizontal alignment");
                    file.Close();
                }
                if (valignProperty != null)
                {
                    //  Create the binding.
                    Binding binding = new Binding
                    {
                        Source = this,
                        Path = new PropertyPath("VerticalAlignment")
                    };

                    //  Bind the child's margin to the grid's padding.
                    BindingOperations.SetBinding(child, VerticalAlignmentProperty, binding);
                }
                else
                {
                    //debug
                    string Path = Environment.CurrentDirectory.Replace(@"bin\Debug", "") + "\\log.txt";
                    StreamWriter file = new StreamWriter(Path);
                    file.WriteLine("No vertical alignment");
                    file.Close();
                } */
                if (marginProperty != null)
                {
                    {
                        //debug

                        string Path = Environment.CurrentDirectory.Replace(@"bin\Debug", "") + "\\log.txt";
                        using (StreamWriter writer = File.AppendText(Path))
                            writer.WriteLine("margin property in PaddedGrid_Loaded is binded to " + child.ToString());
                        //end debug
                    }
                    //  Create the binding.
                    Binding binding = new Binding
                    {
                        Source = this,
                        Path = new PropertyPath("Padding")
                    };

                    //  Bind the child's margin to the grid's padding.
                    BindingOperations.SetBinding(child, MarginProperty, binding);
                }
                else
                {
                    {
                        //debug
                        string Path = Environment.CurrentDirectory.Replace(@"bin\Debug", "") + "\\log.txt";
                        using (StreamWriter writer = File.AppendText(Path))
                            writer.WriteLine("margin property in PaddedGrid_Loaded is NOT binded");
                        //end debug
                    }
                }
            }
        }
        public void updateMargin(DependencyObject element)
        {
            {
                //debug
                string Path = Environment.CurrentDirectory.Replace(@"bin\Debug", "") + "\\log.txt";
                using (StreamWriter writer = File.AppendText(Path))
                    writer.WriteLine("updateMargin is called");
                //end debug
            }
            int childCount = VisualTreeHelper.GetChildrenCount(this);
            for (int i = 0; i < childCount; i++)
            {
                DependencyProperty marginProperty = GetMarginProperty(element);
                
                if (marginProperty != null)
                {
                    {
                        //debug
                        string Path = Environment.CurrentDirectory.Replace(@"bin\Debug", "") + "\\log.txt";
                        using (StreamWriter writer = File.AppendText(Path))
                            writer.WriteLine("margin property in updateMargin is binded");
                        //end debug
                    }
                    //  Create the binding.
                    Binding binding = new Binding
                    {
                        Source = this,
                        Path = new PropertyPath("Padding")
                    };

                    //  Bind the child's margin to the grid's padding.
                    BindingOperations.SetBinding(element, MarginProperty, binding);
                }
                else
                {
                    {
                        //debug
                        string Path = Environment.CurrentDirectory.Replace(@"bin\Debug", "") + "\\log.txt";
                        using (StreamWriter writer = File.AppendText(Path))
                            writer.WriteLine("margin property in updateMargin is NOT binded");
                        //end debug
                    }
                }
            }
        }

        protected static DependencyProperty GetMarginProperty(DependencyObject dependencyObject)
        {
            {
                //debug
                string Path = Environment.CurrentDirectory.Replace(@"bin\Debug", "") + "\\log.txt";
                using (StreamWriter writer = File.AppendText(Path))
                    writer.WriteLine("GetMarginProperty is called");
                //end debug
            }
            //  Go through each property for the object.
            foreach (PropertyDescriptor propertyDescriptor in
                        TypeDescriptor.GetProperties(dependencyObject))
            {
                //  Get the dependency property descriptor.
                DependencyPropertyDescriptor dpd =
                   DependencyPropertyDescriptor.FromProperty(propertyDescriptor);

                //  Have we found the margin?
                if (dpd != null && dpd.Name == "Margin")
                {
                    //  We've found the margin property, return it.
                    return dpd.DependencyProperty;
                }
            }

            //  Failed to find the margin, return null.
            return null;
        }
        protected static DependencyProperty GetHorizontalAlignment(DependencyObject dependencyObject)
        {
            {
                //debug
                string Path = Environment.CurrentDirectory.Replace(@"bin\Debug", "") + "\\log.txt";
                using (StreamWriter writer = File.AppendText(Path))
                    writer.WriteLine("GetHorizontalAlignment is called");
                //end debug
            }
            //  Go through each property for the object.
            foreach (PropertyDescriptor propertyDescriptor in
                        TypeDescriptor.GetProperties(dependencyObject))
            {
                //  Get the dependency property descriptor.
                DependencyPropertyDescriptor dpd =
                   DependencyPropertyDescriptor.FromProperty(propertyDescriptor);

                //  Have we found the margin?
                if (dpd != null && dpd.Name == "HorizontalAlignment")
                {
                    //  We've found the margin property, return it.
                    return dpd.DependencyProperty;
                }
            }

            //  Failed to find the margin, return null.
            return null;
        }
        protected static DependencyProperty GetVerticalAlignment(DependencyObject dependencyObject)
        {
            {
                //debug
                string Path = Environment.CurrentDirectory.Replace(@"bin\Debug", "") + "\\log.txt";
                using (StreamWriter writer = File.AppendText(Path))
                    writer.WriteLine("GetVerticalAlignment is called");
                //end debug
            }
            //  Go through each property for the object.
            foreach (PropertyDescriptor propertyDescriptor in
                        TypeDescriptor.GetProperties(dependencyObject))
            {
                //  Get the dependency property descriptor.
                DependencyPropertyDescriptor dpd =
                   DependencyPropertyDescriptor.FromProperty(propertyDescriptor);

                //  Have we found the margin?
                if (dpd != null && dpd.Name == "VerticalAlignment")
                {
                    //  We've found the margin property, return it.
                    return dpd.DependencyProperty;
                }
            }

            //  Failed to find the margin, return null.
            return null;
        }

        private static void OnPaddingChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs args)
        {
            {
                //debug
                string Path = Environment.CurrentDirectory.Replace(@"bin\Debug", "") + "\\log.txt";
                using (StreamWriter writer = File.AppendText(Path))
                    writer.WriteLine("OnPaddingChanged is called");
                //end debug
            }
            //  Get the padded grid that has had its padding changed.
            PaddedGrid paddedGrid = dependencyObject as PaddedGrid;

            //  Force the layout to be updated.
            paddedGrid.UpdateLayout();
        }
    }
}
