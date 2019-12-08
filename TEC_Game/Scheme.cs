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

        public Node getRoot()
        {
            return nodes.Count > 0 ? nodes[0] : throw new Exception("Sorry not sorry");
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
            int x = node.GetX();
            while ((x <= 70) && (x >= 0))
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
            int y = node.GetY();
            while ((y <= 50) && (y >= 0))
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

        public void RemoveNullor(Grid gameGrid)
        {
            while (FindNorator() != null)
            {
                RemoveElement(FindNorator(), gameGrid);
            }

            while (FindNullator() != null)
            {
                RemoveElement(FindNullator(), gameGrid);
            }
        }

        public void RemoveElement(BaseElement element, Grid gameGrid)
        {
            elements.Remove(element);
            nodes[element.GetNode1().GetId() - 1].RemoveElement(element);
            nodes[element.GetNode2().GetId() - 1].RemoveElement(element);

            if (elements.Count > 1)
            {
                if (element.GetNode1().GetConnectedElements()[0] is NullorElement &&
                    element.GetNode1().GetConnectedElementsCount() == 1)
                {
                    element.GetNode1().GetConnectedElements()[0].ChangeNode(element.GetNode1(), element.GetNode2());
                }

                if (element.GetNode2().GetConnectedElements()[0] is NullorElement &&
                    element.GetNode2().GetConnectedElementsCount() == 1)
                {
                    element.GetNode2().GetConnectedElements()[0].ChangeNode(element.GetNode2(), element.GetNode1());
                }
            }

            if (element.GetNode1().GetConnectedElementsCount() == 0)
            {
                gameGrid?.Children.Remove(element.GetNode1());
                RemoveNode(element.GetNode1());
            }

            if (element.GetNode2().GetConnectedElementsCount() == 0)
            {
                gameGrid?.Children.Remove(element.GetNode2());
                RemoveNode(element.GetNode2());
            }
            element.Destroy();
            element = null;
        }

        public void RemoveNode(Node node)
        {
            for (int i = node.GetId(); i < nodes.Count; i++)
            {
                nodes[i].SetId(i);
            }
            nodes.Remove(node);
            node = null;
        }

        public void RemoveWire(Wire wire)
        {
            wires.Remove(wire);
            wire.Destroy();
            wire = null;
        }
    }
}
