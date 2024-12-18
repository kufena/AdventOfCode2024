using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class UPQTests
    {
        public void Tests()
        {
            Test1();
            Test2();
            Test3();
            Test4();
        }

        public void Test1()
        {
            UpdatablePriorityQueue upq = new();
            upq.Enqueue((1, 2), 4);
            upq.Enqueue((2, 3), 5);
            upq.Enqueue((4, 5), 2);

            ((int, int) n1, int p1) = upq.Dequeue();
            ((int, int) n2, int p2) = upq.Dequeue();
            ((int, int) n3, int p3) = upq.Dequeue();

            Console.WriteLine($"{p1} {p2} {p3}");
        }
        public void Test2()
        {
            (int, int) node1 = (1, 2);
            (int, int) node2 = (2, 3);
            (int, int) node3 = (4, 5);

            UpdatablePriorityQueue upq = new();
            upq.Enqueue(node1, 4);
            upq.Enqueue(node2, 5);
            upq.Enqueue(node3, 2);

            upq.UpdatePriority(node1, 1);

            ((int, int) n1, int p1) = upq.Dequeue();
            ((int, int) n2, int p2) = upq.Dequeue();
            ((int, int) n3, int p3) = upq.Dequeue();

            Console.WriteLine($"{p1} {p2} {p3}");
        }
        public void Test3()
        {
            (int, int) node1 = (1, 2);
            (int, int) node2 = (2, 3);
            (int, int) node3 = (4, 5);

            UpdatablePriorityQueue upq = new();
            upq.Enqueue(node2, 5);
            upq.Enqueue(node1, 4);
            upq.Enqueue(node3, 2);
            upq.UpdatePriority(node1, 1);
            upq.Enqueue((1, 9), 0);

            ((int, int) n1, int p1) = upq.Dequeue();
            ((int, int) n2, int p2) = upq.Dequeue();
            ((int, int) n3, int p3) = upq.Dequeue();

            upq.Enqueue((3, 3), 9);
            upq.Enqueue((1, 1), 7);

            Console.WriteLine($"{p1} {p2} {p3}");
            while (upq.Count() > 0)
            {
                (_, int pr) = upq.Dequeue();
                Console.WriteLine(pr);
            }

        }
        public void Test4()
        {
            (int, int) node1 = (1, 2);
            (int, int) node2 = (2, 3);
            (int, int) node3 = (4, 5);

            UpdatablePriorityQueue upq = new();
            upq.Enqueue(node1, 4);
            upq.Enqueue(node2, 5);
            upq.Enqueue(node3, 2);

            upq.UpdatePriority(node1, 1);

            ((int, int) n1, int p1) = upq.Dequeue();
            ((int, int) n2, int p2) = upq.Dequeue();
            ((int, int) n3, int p3) = upq.Dequeue();

            Console.WriteLine($"{p1} {p2} {p3}");
        }

    }
}
