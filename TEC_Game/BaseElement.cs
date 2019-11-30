using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace tec
{
    abstract class BaseElement
    {
        protected Image image;
        private Node node1, node2;
        private int id;

        protected BaseElement(Node node1, Node node2, int id)
        {
            image = null;
            this.node1 = node1;
            this.node2 = node2;
            this.id = id;
        }

        public Node GetNode1()
        {
            return node1;
        }

        public Image GetImage()
        {
            return image;
        }

        public Node GetNode2()
        {
            return node2;
        }

        public int GetId()
        {
            return id;
        }

        public void Destroy()
        {
            image = null;
        }
        public abstract void ChangeImageDirectionToLand();
    }
}
