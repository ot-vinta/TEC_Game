using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tec
{
    abstract class BaseElement
    {
        private Node node1, node2;
        private int id;

        protected BaseElement(Node node1, Node node2, int id)
        {
            this.node1 = node1;
            this.node2 = node2;
            this.id = id;
        }

        public Node GetNode1()
        {
            return node1;
        }

        public Node GetNode2()
        {
            return node2;
        }

        public int GetId()
        {
            return id;
        }

        public abstract void Destroy();
    }
}
