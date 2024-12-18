using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class UpdatablePriorityQueue
    {
        Dictionary<(int, int), int> nodesToPriorities = new();
        Dictionary<(int, int), Link> nodesToLink = new();

        Link? top = null;
        Link? bottom = null;

        public int Count()
        {
            return nodesToPriorities.Count;
        }

        public void UpdatePriority((int, int) n, int p)
        {
            if (nodesToPriorities.ContainsKey(n))
            {
                nodesToPriorities[n] = p;
                var l = nodesToLink[n];
                RemoveFromList(l);
                InsertIntoList(p, l);
                SanityCheck();
            }
            else
            {
                SanityCheck();
                throw new Exception("updating priority on Node that isn't there.");
            }
        }

        private void SanityCheck()
        {
            //var ptr = top;
            //HashSet<(int, int)> found = new();
            //while (ptr != null)
            //{
            //    found.Add((ptr.n.Item1, ptr.n.Item2));
            //    ptr = ptr.next;
            //}
            //if (found.Count != nodesToPriorities.Count)
            //{
            //    Console.WriteLine("we're missing a node.");
            //    foreach (var nnn in nodesToPriorities.Keys)
            //    {
            //        if (!found.Contains(nnn))
            //        {
            //            Console.WriteLine($"Well -- {nnn.Item1}, {nnn.Item2}");
            //        }
            //    }
            //}
        }

        private void InsertIntoList(int priority, Link l)
        {
            if (top == null)
            {
                top = l;
                bottom = l;
                return;
            }

            if (bottom == null)
            {
                throw new Exception("How can top be not null but bottom be null?");
            }

            if (nodesToPriorities[bottom.n] <= priority) // stick it on the end.
            {
                l.prev = bottom;
                bottom.next = l;
                bottom = l;
                return;
            }

            Link? t = top;

            while (nodesToPriorities[t.n] < priority)
            {
                t = t.next;
            }
            // insert above t.
            var trev = t.prev;
            l.next = t;
            l.prev = trev;
            t.prev = l;
            if (trev == null) // t is top.
            {
                top = l;
            }
            else
            {
                trev.next = l;
            }
        }

        private void RemoveFromList(Link l)
        {
            if (l == top)
            {
                Link? newt = top.next;
                if (newt == null) // that's it!
                {
                    top = null;
                    bottom = null;
                    return;
                }
                else
                {
                    newt.prev = null;
                    top = newt;
                    return;
                }

            }

            if (l == bottom)
            {
                bottom = bottom.prev;
                if (bottom == null) throw new Exception("unexpect null bottom.");
                bottom.next = null;
                return;
            }

            var lext = l.next;
            var lrev = l.prev;
            if (l.next == null) throw new Exception("unexpect null next here.");
            l.next.prev = lrev;
            if (l.prev == null) throw new Exception("unexpect null prev here.");
            l.prev.next = lext;

        }

        public void Enqueue((int, int) node, int priority)
        {
            Link l = new Link(node);

            nodesToLink.Add(node, l);
            nodesToPriorities.Add(node, priority);

            InsertIntoList(priority, l);
            SanityCheck();
        }

        public ((int, int), int) Dequeue()
        {
            if (top == null)
                throw new Exception("nowt to dequeue here.");

            (int, int) result = top.n;
            int priority = nodesToPriorities[result];
            nodesToPriorities.Remove(result);
            nodesToLink.Remove(result);

            Link? newt = top.next;

            if (newt == null)
            {
                top = null;
                bottom = null;
                SanityCheck();
                return (result, priority);
            }

            top = newt;
            newt.prev = null;
            SanityCheck();
            return (result, priority);
        }

        (int, int) Peek()
        {
            if (top == null) throw new Exception("wow");
            return top.n;
        }


        private class Link
        {
            public Link? prev { get; set; }
            public Link? next { get; set; }

            public (int, int) n { get; set; }
            public Link((int, int) m)
            {
                n = m;
                next = null;
                prev = null;
            }

        }
    }
}

