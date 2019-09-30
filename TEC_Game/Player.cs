using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tec
{
    class Player
    {
        private Button nodeChosen1, nodeChosen2;

        public Player()
        {
            nodeChosen1 = null;
            nodeChosen2 = null;
        }

        public void ChooseNode(Button node)
        {
            if (nodeChosen1 == null)
                nodeChosen1 = node;
            else
                nodeChosen2 = node;
        }

        public Button GetNodeChosen1()
        {
            return nodeChosen1;
        }

        public Button GetNodeChosen2()
        {
            return nodeChosen2;
        }

        public bool NodesChosen()
        {
            return ((nodeChosen1 != null) && (nodeChosen2 != null));
        }
    }
}
