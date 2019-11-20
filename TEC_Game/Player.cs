using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace tec
{
    class Player
    {
        private Node nodeChosen1, nodeChosen2;

        public Player()
        {
            nodeChosen1 = null;
            nodeChosen2 = null;
        }

        public void ChooseNode(Node node)
        {
            if (nodeChosen1 == null)
                nodeChosen1 = node;
            else
                nodeChosen2 = node;
        }

        public void RemoveNode(Node node)
        {
            if (nodeChosen1 == node)
            {
                nodeChosen1 = null;
            }
            else
            {
                nodeChosen2 = null;
            }
        }

        public Node GetNodeChosen1()
        {
            return nodeChosen1;
        }

        public Node GetNodeChosen2()
        {
            return nodeChosen2;
        }

        public bool NodesChosen()
        {
            return ((nodeChosen1 != null) && (nodeChosen2 != null));
        }
    }
}
