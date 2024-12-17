using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Day16
{
    public class UpdatablePriorityQueue
    {
        Dictionary<Node, int> nodesToPriorities = new();
        Link? top = null;
        Link? bottom = null;

        void Enqueue(Node node, int priority)
        {
            Link l = new Link(node);
            nodesToPriorities.Add(node, priority);

            if (top == null) 
            {
                top = l;
                bottom = l;
                return;
            }

            if (nodesToPriorities[bottom.n] <= priority) // stick it on the end.
            {
                l.prev = bottom;
                bottom.next = l;
                bottom = l;
            }
            
            Link t = top;
            
            while (nodesToPriorities[t.n] < priority)
            {
                t = t.next;
            }
            if (t == null)
        }



        private class Link
        {
            public Link? prev { get; set; }
            public Link? next { get; set; }

            public Node n { get; set; }
            public Link(Node m) 
            {
                n = m;
                next = null;
                prev = null;
            }

        }
    }
}
