using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Xml.Serialization;
using TEC_Game;

namespace tec
{
    class Scheme
    {
        private List<BaseElement> elements;
        private List<Node> nodes;
        private List<Wire> wires;

        public Scheme()
        {
            elements = new List<BaseElement>();
            nodes = new List<Node>();
            wires = new List<Wire>();
        }

        public bool HasNullor()
        {
            bool hasNullator = false;
            bool hasNorator = false;
            foreach (var element in elements)
            {
                if (element is Nullator) hasNullator = true;
                if (element is Norator) hasNorator = true;
            }
            return ((hasNullator == true) && (hasNorator == true));
        }

        public bool SchemeIsConnected()
        {
            foreach (var element in elements)
            {
                if (element.GetNode2() == null)
                    return false;
            }

            return true;
        }

        public Nullator FindNullator()
        {
            foreach (var element in elements)
            {
                if (element is Nullator)
                    return (Nullator) element;
            }

            return null;
        }

        public Norator FindNorator()
        {
            foreach (var element in elements)
            {
                if (element is Norator)
                    return (Norator)element;
            }

            return null;
        }

        public int GetNodeConnectionsCount(Node node)
        {
            return node.GetConnectedElementsCount();
        }

        public int GetElementsSize()
        {
            return elements.Count();
        }

        public int GetNodesCount()
        {
            return nodes.Count;
        }

        public void AddElement(BaseElement element)
        {
            elements.Add(element);
            nodes[element.GetNode1().GetId() - 1].AddConnectedElement(element);
            nodes[element.GetNode2().GetId() - 1].AddConnectedElement(element);
        }

        public void AddNode(Node node)
        {
            nodes.Add(node);
        }

        public void AddWire(Wire wire)
        {
            wires.Add(wire);
        }

        public Node GetNode(int id)
        {
            foreach (var node in nodes)
            {
                if (node.GetId() == id)
                    return node;
            }

            return null;
        }

        public Node GetNode(int X, int Y)
        {
            foreach (var node in nodes)
            {
                if ((node.GetX() == X) && (node.GetY() == Y))
                    return node;
            }

            return null;
        }

        public Node GetHorizontalNode(Node node, int step)
        {
            step = step < 0 ? 1 : -1;
            int x = node.GetX();
            while ((x <= 40) && (x >= 0))
            {
                x += step;
                if (GetNode(x, node.GetY()) != null)
                {
                    return GetNode(x, node.GetY());
                }
            }

            return null;
        }

        public Node GetVerticalNode(Node node, int step)
        {
            step = step < 0 ? 1 : -1;
            int y = node.GetY();
            while ((y <= 40) && (y >= 0))
            {
                y += step;
                if (GetNode(node.GetX(), y) != null)
                {
                    return GetNode(node.GetX(), y);
                }
            }

            return null;
        }

        public int GetWiresCount()
        {
            return wires.Count;
        }

        public void RemoveElement(BaseElement element)
        {
            elements.Remove(element);
            nodes[element.GetNode1().GetId() - 1].RemoveElement(element);
            nodes[element.GetNode2().GetId() - 1].RemoveElement(element);
            element.Destroy();
            element = null;
        }
    }
}
