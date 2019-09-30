using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace tec
{
    abstract class NullorElement : BaseElement
    {
        private string direction; //TO DO
        protected NullorElement(Node node1, Node node2, int id) : base(node1, node2, id)
        {
        }

        public string GetDirection()
        {
            return direction;
        }
    }
}
