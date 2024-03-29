﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TEC_Game;

namespace tec
{
    class Node : Button, ICloneable
    {
        private int id, X, Y;
        private List<BaseElement> connectedElements;
        private HashSet<Wire> connectedWires;

        public Node(int id, int X, int Y) : base()
        {
            VerticalAlignment = VerticalAlignment.Stretch;
            HorizontalAlignment = HorizontalAlignment.Stretch;

            Background = Brushes.Black;

            connectedElements = new List<BaseElement>();
            connectedWires = new HashSet<Wire>();
            this.id = id;
            this.X = X;
            this.Y = Y;
        }

        public void AddConnectedWire(Wire wire)
        {
            connectedWires.Add(wire);
        }

        public void RemoveConnectedWire(Wire wire)
        {
            connectedWires.Remove(wire);
        }

        public int GetWiresCount()
        {
            return connectedWires.Count;
        }

        public bool HasWire(Wire wire)
        {
            return connectedWires.Contains(wire) ? true : false;
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

        public List<BaseElement> GetConnectedElements()
        {
            return connectedElements;
        }

        public HashSet<Wire> GetConnectedWires()
        {
            return connectedWires;
        }

        public void RemoveElement(BaseElement element)
        {
            connectedElements.Remove(element);
        }

        public int GetX()
        {
            return X;
        }

        public int GetY()
        {
            return Y;
        }

        public int GetId()
        {
            return id;
        }

        public NullorElement GetNullorElement()
        {
            foreach (var element in connectedElements)
            {
                if (element is NullorElement)
                    return (NullorElement) element;
            }

            return null;
        }

        public object Clone()
        {
            return new Node(id, X, Y) { id = this.id, X = this.X, Y = this.Y, connectedElements = this.connectedElements};
        }
    }
}
