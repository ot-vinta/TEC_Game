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
        private int elementMaxId;
        private List<Node> nodes;
        private int nodeMaxId;
        private List<Wire> wires;
        private int wireMaxId;

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
                if (element != null)
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

        public int GetElementMaxId()
        {
            return elementMaxId;
        }

        public int GetNodesCount()
        {
            return nodes.Count;
        }

        public int GetNodeMaxId()
        {
            return nodeMaxId;
        }

        public void AddElement(BaseElement element)
        {
            elements.Add(element);
            elementMaxId++;
            nodes[element.GetNode1().GetId() - 1].AddConnectedElement(element);
            nodes[element.GetNode2().GetId() - 1].AddConnectedElement(element);
        }

        public void AddNode(Node node)
        {
            nodes.Add(node);
            nodeMaxId++;
        }

        public void AddWire(Wire wire)
        {
            wires.Add(wire);
            wireMaxId++;
        }

        public void AddWireToObject(Wire wire, object obj)
        {
            switch (obj)
            {
                case Node temp1:
                    temp1.AddConnectedWire(wire);
                    wire.AddConnectedObject(temp1);
                    break;
                case BaseElement temp:
                    temp.AddWire(wire);
                    wire.AddConnectedObject(temp);
                    break;
                case Wire temp:
                    temp.AddConnectedObject(wire);
                    wire.AddConnectedObject(temp);
                    break;
            }
        }

        public Wire GetWire(int id)
        {
            foreach (var wire in wires)
                if (wire != null)
                {
                    if (wire.GetId() == id)
                        return wire;
                }

            return null;
        }

        public BaseElement GetElement(int id)
        {
            foreach (var element in elements)
                if (element != null)
                {
                    if (element.GetId() == id)
                        return element;
                }

            return null;
        }

        public Node GetNode(int id)
        {
            foreach (var node in nodes)
                if (node != null)
                {
                    if (node.GetId() == id)
                        return node;
                }

            return null;
        }

        public Node GetNode(int X, int Y)
        {
            foreach (var node in nodes)
                if (node != null)
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

        public int GetWireMaxId()
        {
            return wireMaxId;
        }

        public List<Wire> GetWires()
        {
            return wires;
        }

        public List<Node> GetNodes()
        {
            return nodes;
        }

        public void RemoveNullor(Grid gameGrid, Norator norator, Nullator nullator)
        {
            if (norator != null)
            {
                RemoveElement(norator, gameGrid);
            }

            if (nullator != null)
            {
                RemoveElement(nullator, gameGrid);
            }
        }

        public void RemoveElement(BaseElement element, Grid gameGrid)
        {
            elements[element.GetId() - 1] = null;
            int tempId = element.GetId();
            nodes[element.GetNode1().GetId() - 1].RemoveElement(element);
            nodes[element.GetNode2().GetId() - 1].RemoveElement(element);
            element.GetWire1().RemoveObject(element);
            element.GetWire2().RemoveObject(element);

            if (elements.Count > 1 && element is Resistor)
            {
                if (element.GetNode1().GetConnectedElementsCount() == 1 &&
                    element.GetNode1().GetConnectedElements()[0] is NullorElement)
                {
                    element.GetNode1().GetConnectedElements()[0].ChangeNode(element.GetNode1(), element.GetNode2());
                }

                if (element.GetNode2().GetConnectedElementsCount() == 1 && 
                    element.GetNode2().GetConnectedElements()[0] is NullorElement)
                {
                    element.GetNode2().GetConnectedElements()[0].ChangeNode(element.GetNode2(), element.GetNode1());
                }
            }

            if (elements.Count > 1 && element is Conductor)
            {
                int conductorsCount = 0;
                foreach (var elem in elements)
                    if (elem != null)
                    {
                        if (elem.GetNode1() == element.GetNode1() && elem.GetNode2() == element.GetNode2() ||
                            elem.GetNode2() == element.GetNode1() && elem.GetNode1() == element.GetNode2())
                            conductorsCount++;
                    }

                if (conductorsCount == 1)
                {
                    Node tempNode1 = null, tempNode2 = null;

                    foreach (var wire in element.GetNode1().GetConnectedWires())
                    {
                        if (wire.GetObject1() is Node && wire.GetObject2() is Node)
                        {
                            tempNode1 = (Node)wire.GetObject1();
                            tempNode2 = (Node)wire.GetObject2();

                            CheckForFreeNodes(tempNode1, tempNode2);
                        }
                    }

                    tempNode1 = null;
                    tempNode2 = null;

                    foreach (var wire in element.GetNode2().GetConnectedWires())
                    {
                        if (wire.GetObject1() is Node && wire.GetObject2() is Node)
                        {
                            tempNode1 = (Node)wire.GetObject1();
                            tempNode2 = (Node)wire.GetObject2();

                            CheckForFreeNodes(tempNode1, tempNode2);
                        }
                    }
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

        private void CheckForFreeNodes(Node tempNode1, Node tempNode2)
        {
            if (!(tempNode1?.GetConnectedElementsCount() > 0) || !(tempNode2?.GetConnectedElementsCount() > 0)) return;
            if (tempNode1.GetConnectedElements()[0] is NullorElement &&
                tempNode1.GetConnectedElementsCount() == 1)
            {
                tempNode1.GetConnectedElements()[0].ChangeNode(tempNode1, tempNode2);
            }

            if (tempNode2.GetConnectedElements()[0] is NullorElement &&
                tempNode2.GetConnectedElementsCount() == 1)
            {
                tempNode2.GetConnectedElements()[0].ChangeNode(tempNode2, tempNode1);
            }
        }

        public void RemoveNode(Node node)
        {
            nodes[node.GetId() - 1] = null;
            node = null;
        }

        public void RemoveWire(Wire wire)
        {
            wires[wire.GetId() - 1] = null;
            wire.Destroy();
            wire = null;
        }
    }
}
