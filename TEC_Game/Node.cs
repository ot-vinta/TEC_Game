using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tec
{
    class Node
    {
        private Button button;
        private int id;

        public Node(Button button, int id)
        {
            this.button = button;
            this.id = id;
        }

        public int GetId()
        {
            return id;
        }

        public Button GetButton()
        {
            return button;
        }
    }
}
