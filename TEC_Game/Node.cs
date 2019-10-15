using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace tec
{
    class Node
    {
        private Button button;
        private int id, X, Y;
        private List<BaseElement> connectedElements;

        public Node(Button button, int id, int X, int Y)
        {
            this.button = button;
            button.Height = 25;
            button.Width = 25;

            button.VerticalAlignment = VerticalAlignment.Top;
            button.HorizontalAlignment = HorizontalAlignment.Left;

            button.Background = new SolidColorBrush(Colors.Black);

            button.Background = new SolidColorBrush(Colors.Black);

            connectedElements = new List<BaseElement>();
            this.id = id;
            this.X = X;
            this.Y = Y;
        }

        public void AddConnectedElement(BaseElement element)
        {
            connectedElements.Add(element);
        }

        public Resistor GetResistor()
        {
            if ((connectedElements.Count == 1) && (connectedElements[0] is Resistor))
                return (Resistor) connectedElements[0];
            else
                return null;
        }

        public Conductor GetConductor()
        {
            foreach (var element in connectedElements)
            {
                if (element is Conductor)
                    return (Conductor) element;
            }
            return null;
        }

        public int GetConnectedElementsCount()
        {
            return connectedElements.Count;
        }

        public void RemoveElement(BaseElement element)
        {
            connectedElements.Remove(element);
        }

        public int GetId()
        {
            return id;
        }

        public Button GetButton()
        {
            return button;
        }
    }
}
