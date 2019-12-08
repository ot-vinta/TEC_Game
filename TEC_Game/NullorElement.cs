using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace tec
{
    abstract class NullorElement : BaseElement
    {
        private string direction;
        protected NullorElement(Node node1, Node node2, int id) : base(node1, node2, id)
        {
            direction = "right";
        }

        public string GetDirection()
        {
            return direction;
        }

        public void SetDirection(string newDirection)
        {
            direction = newDirection;
            RotateTransform rotate = new RotateTransform(180);
            image.RenderTransform = rotate;
        }
    }
}
