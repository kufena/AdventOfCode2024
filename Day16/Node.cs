using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day16
{
    public class Node : IEquatable<Node>
    {
        public int col { get; set; }
        public int row { get; set; }

        public int counter { get; set; }
        public List<Node> edges { get; set; }

        public Node(int r, int c)
        {
            row = r;
            col = c;
        }

        public override bool Equals(object? obj)
        {
            Node other = obj as Node;
            return (other.col == col && other.row == row);
        }

        public void Reset() => counter = 0;
        public Node NextEdge()
        {
            if (counter >= edges.Count)
                return null;
            Node n = edges[counter];
            counter++;
            return n;
        }

        public bool Equals(Node? other)
        {
            if (other == null) return false;
            return (other.col == col && other.row == row);
        }
    }
}
