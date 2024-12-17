using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day16
{
    internal class Edge
    {
        Node a { get; set; }
        Node b { get; set; }

        public Edge(Node one, Node two)
        {
            a = one;
            b = two;
        }
    }
}
