using System;
using System.Collections.Generic;
using System.Linq;
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
