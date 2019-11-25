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
        }
        void PaddedGrid_Loaded(object sender, RoutedEventArgs e)
        {
            //  Get the number of children.
            int childCount = VisualTreeHelper.GetChildrenCount(this);

            //  Go through the children.
            for (int i = 0; i < childCount; i++)
            {
                //  Get the child.
                DependencyObject child = VisualTreeHelper.GetChild(this, i);

                //  Try and get the margin property.
                DependencyProperty marginProperty = GetMarginProperty(child);


                //  If we have a margin property, bind it to the padding.
                if (marginProperty != null)
                {
                    //  Create the binding.
                    Binding binding = new Binding
                    {
                        Source = this,
                        Path = new PropertyPath("Padding")
                    };

                    //  Bind the child's margin to the grid's padding.
                    BindingOperations.SetBinding(child, marginProperty, binding);
                }
            }
        }
        protected static DependencyProperty GetMarginProperty(DependencyObject dependencyObject)
        {
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

        private static void OnPaddingChanged(DependencyObject dependencyObject,
               DependencyPropertyChangedEventArgs args)
        {
            //  Get the padded grid that has had its padding changed.
            PaddedGrid paddedGrid = dependencyObject as PaddedGrid;

            //  Force the layout to be updated.
            paddedGrid.UpdateLayout();
        }
    }
}
