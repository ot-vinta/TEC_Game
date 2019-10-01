using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace tec
{
    class Node
    {
        private Button button;
        private int id;
        private List<BaseElement> connectedElements;

        public Node(Button button, int id)
        {
            this.button = button;
            connectedElements = new List<BaseElement>();
            this.id = id;
        }

        public void AddConnectedElement(BaseElement element)
        {
            connectedElements.Add(element);
        }

        public BaseElement GetConnectedElement()
        {
            if (connectedElements.Count == 1)
                return connectedElements[0];
            else
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
