using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace tec
{
    class Resistor : BaseElement
    {
        //Make smth with Image
        public Resistor(Node node1, Node node2, int id) : base(node1, node2, id)
        {
        }

        public override void Destroy()
        {
            //TO DO
        }
    }
}
