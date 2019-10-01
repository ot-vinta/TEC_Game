using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace tec
{
    class Scheme
    {
        private List<BaseElement> elements;

        public Scheme()
        {
            elements = new List<BaseElement>();
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
            int[] nodeFreq = new int[GetElementsSize()];
            foreach (var element in elements)
            {
                nodeFreq[element.GetNode1().GetId()]++;
                nodeFreq[element.GetNode2().GetId()]++;
            }

            for (int i = 1; i < GetElementsSize(); i++)
            {
                if (nodeFreq[i] == 1)
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

        public int GetElementsSize()
        {
            return elements.Count();
        }

        public void AddElement(BaseElement element)
        {
            elements.Add(element);
        }

        public void RemoveElement(BaseElement element)
        {
            elements.Remove(element);
            element.Destroy();
            element = null;
        }
    }
}
