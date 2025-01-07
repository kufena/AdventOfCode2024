// See https://aka.ms/new-console-template for more information

using Day16;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using Utilities;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);
int rows = lines.Length;
int cols = lines[0].Length;

int startcol = 0;
int startrow = 0;
int endcol = 0;
int endrow = 0;

HashSet<(int,int)> mazeblockers = new();
Dictionary<(int, int), Node> graphNodes = new();
Dictionary<Node, (int,int)> nodes = new();
Dictionary<(int, int), HashSet<(int, int)>> edges = new();

for (int i = 0; i < rows; i++) {
    for (int j = 0; j < cols; j++) {
        if (lines[i][j] == '#')
        {
            mazeblockers.Add((i, j));
            continue;
        }
        if (lines[i][j] == 'S')
        {
            startrow = i;
            startcol = j;
        }
        if (lines[i][j] == 'E')
        {
            endrow = i;
            endcol = j;
        }
        Node n = new Node(i, j);
        graphNodes.Add((i, j), n);
        nodes.Add(n, (i, j));
    }
}

foreach (var node in nodes)
{
    (int r, int c) = node.Value;
    HashSet<(int, int)> hs = new();
    edges.Add((r, c), hs);
    if (graphNodes.ContainsKey((r - 1, c)))
        hs.Add((r - 1, c));    
    if (graphNodes.ContainsKey((r + 1, c)))
        hs.Add((r + 1, c));
    if (graphNodes.ContainsKey((r, c + 1)))
        hs.Add((r, c + 1));
    if (graphNodes.ContainsKey((r, c - 1)))
        hs.Add((r, c - 1));
    node.Key.edges = new List<Node>();
    foreach (var p in hs) node.Key.edges.Add(graphNodes[p]);
}

var tests = new UPQTests();
tests.Tests();

//ShortestPath(nodes, graphNodes, startrow, startcol, endrow, endcol, true);
int maxlen = ShortestPath(nodes, graphNodes, startrow, startcol, endrow, endcol, true);
var points = NodesOnPaths(nodes, graphNodes, startrow, startcol, endrow, endcol, maxlen);
Console.WriteLine(points.Count);

//
// There's a notion here of the nodes being a row, column and direction. And hence,
// the edges and their weights represent the change or lack of direction change.
// I'm not sure I've captured that here very well.
//
// It worked, anyway.
int ShortestPath(Dictionary<Node, (int, int)> nodes, Dictionary<(int, int), Node> graphNodes, 
                  int startrow, int startcol, int endrow, int endcol,
                  bool part1)
{
    UpdatablePriorityQueue upq = new();
    Dictionary<Node, int> distance = new();
    Dictionary<Node, Node?> prev = new();

    Node target = graphNodes[(endrow, endcol)];

    foreach (var nod in nodes.Keys)
    {
        distance[nod] = int.MaxValue;
        prev[nod] = null;
        upq.Enqueue((nod.row,nod.col), int.MaxValue);
    }
    var startNode = graphNodes[(startrow, startcol)];
    distance[startNode] = 0;
    upq.UpdatePriority((startNode.row, startNode.col), 0);

    while (upq.Count() > 0)
    {
        ((int,int) coord, int udistance) = upq.Dequeue();
        Node u = graphNodes[coord];
        foreach (var v in u.edges)
        {
            if (v == u) continue;
            var uprev = prev[u];
            if (uprev == null)
                uprev = u;
            //
            // Change call to dirChangeCost(...) to 1 to get a basic
            // shortest path count.
            //
            int stepdist = (int) (part1 ? dirChangeCost((uprev.row, uprev.col), coord, (v.row, v.col)) : 1);
            int alt = udistance + stepdist;
            if (alt >= 0 && alt < distance[v]) // if we have a negative new dist then don't put that in.
            {
                if (alt < 0)
                    Console.WriteLine("ohdear");
                distance[v] = alt;
                prev[v] = u;
                upq.UpdatePriority((v.row, v.col), alt);
            }
        }
    }

    Console.WriteLine($"Shortest appears to be {distance[target]}");
    Node prv = prev[target];
    List<(int, int)> steps = new List<(int, int)>() { (endrow, endcol) };
    while (prv != startNode)
    { 
        steps.Add((prv.row, prv.col));
        prv = prev[prv];
    }
    steps.Reverse();
    string strpath = pathToString(steps);
    Console.WriteLine(strpath);
    long finaldist = prefixToCount(strpath);
    // HACK HACK HACK HACK HACK HACK HACK HACK HACK HACK HACK HACK HACK
    // Because of the way the first step works, it always gives a number
    // that's out by 1, so we compensate here.
    //
    // OK, this is wrong but the above distance[target] is always correct,
    // so that's ok.
    Console.WriteLine(finaldist+1);
    Console.WriteLine($"What now?");

    return distance[target]; // distance[target];
}

HashSet<(int,int)> NodesOnPaths(Dictionary<Node, (int, int)> nodes, Dictionary<(int, int), Node> graphNodes,
                  int startrow, int startcol, int endrow, int endcol, int maxlen)
{
    HashSet<(int,int)> points = new HashSet<(int, int)>();
    int counter = 1;

    PriorityQueue<(List<(int, int)>, long, string), long> workPQueue = new();
    workPQueue.Enqueue((new List<(int, int)>() { (startrow, startcol) }, 0, ">"), 0);

    HashSet<(int, int, string)> memos = new();

    while (workPQueue.Count > 0)
    {

        if (counter % 1000000 == 0) 
            Console.WriteLine($"{counter} {workPQueue.Count}");
        counter++;

        (List<(int, int)> current, long cost, string direction) = workPQueue.Dequeue();

        if (cost > maxlen)
            continue; // we know the shortest already.

        (int currow, int curcol) = current.Last();
        if (currow == endrow && curcol == endcol && cost == maxlen)
        {
            Console.WriteLine($"Found one of {current.Count} points.");
            foreach (var p in current)
            {
                points.Add(p);
            }
        }
        else
        {
            memos.Add((currow, curcol, direction));
            var nexts = edges[(currow, curcol)];
            Dictionary<string, (List<(int, int)>, long, string)> toAdd = new();
            foreach (var npoint in nexts)
            {
                if (!current.Contains(npoint)) // no loopy loops
                {
                    (long newcostdelta, string newdirection) = GenerateNewCost(currow, curcol, npoint.Item1, npoint.Item2, direction);
                    if (cost + newcostdelta > maxlen) // don't bother.
                        continue;
                    //                    string newprefix = $"{prefix}{newdirection}";
                    if (memos.Contains((npoint.Item1, npoint.Item2, newdirection)))
                    {
                        //Console.WriteLine($"seen it - {(npoint.Item1, npoint.Item2, newdirection, cost + newcostdelta)}");
                        continue;
                    }

                    List<(int, int)> curcopy = new();
                    foreach (var cc in current) curcopy.Add(cc);
                    curcopy.Add(npoint);
                    toAdd.Add(newdirection, (curcopy, cost + newcostdelta, newdirection));
                }
            }
            // This bit is no longer strictly necessary now we use a priority queue,
            // prioritized on the cost.
            if (toAdd.ContainsKey(direction)) // least cost, add first.
            {
                (List<(int, int)> nlist, long ncost, string ndir) = toAdd[direction];
                workPQueue.Enqueue((nlist, ncost, ndir), ncost);
            }
            foreach (var k in toAdd.Keys)
            {
                if (k != direction)
                {
                    (List<(int, int)> nlist, long ncost, string ndir) = toAdd[k];
                    workPQueue.Enqueue((nlist, ncost, ndir), ncost);
                }
            }
        }

    }

    return points;
}

(long newcostdelta, string newdirection) GenerateNewCost(int currow, int curcol, int newrow, int newcol, string direction)
{
    string newdirection = "";
    long newcostdelta = 0;
    if (currow == newrow)
        newdirection = newcol < curcol ? "<" : ">";
    else
        newdirection = newrow < currow ? "^" : "v";
    if (newdirection == direction)
        newcostdelta = 1;
    else
        newcostdelta = 1001;
    return (newcostdelta, newdirection);
}

string pathToString(List<(int,int)> path)
{
    StringBuilder buf = new();
    int c = path.Count;
    string dir = ">";
    buf.Append(dir);
    for (int k = 1; k < c; k++)
    {
        buf.Append(dirChange(path[k - 1], path[k]));
    }

    return buf.ToString();
}

string dirChange((int,int) a, (int,int) b)
{
    (int arow, int acol) = a;
    (int brow, int bcol) = b;
    if (arow == brow && acol < bcol) return ">";
    if (arow == brow && acol > bcol) return "<";
    if (arow < brow && acol == bcol) return "v";
    if (arow > brow && acol == bcol) return "^";
    //throw new ArgumentException($"funny pairs {arow} {acol} to {brow} {bcol}");
    Console.WriteLine($"funny pairs {arow} {acol} to {brow} {bcol}");
    return ">";
}

long dirChangeCost((int, int) a, (int, int) b, (int,int) c)
{
    (int arow, int acol) = a;
    (int brow, int bcol) = b;
    var pair = $"{dirChange(a, b)}{dirChange(b, c)}";
    return prefixToCost(pair);
}

string FindPaths(string prefix, string direction, int startrow, int startcol, int endrow, int endcol, HashSet<(int, int)> mazeblockers, Dictionary<(int, int), string> memo, HashSet<(int, int)> seen)
{
    if (startrow == endrow && startcol == endcol) // we've made it!
    {
        Console.WriteLine($"We have a path of length {prefixToCount(prefix)}");
        Console.WriteLine(prefix);
        return "";
    }

    if (seen.Contains((startrow, startcol))) // loop
        return "FAIL";

    //if (memo.ContainsKey((startrow, startcol)))
    //    return memo[(startrow, startcol)];

    if (mazeblockers.Contains((startrow, startcol)))
    {
        //memo.Add((startrow, startcol), "FAIL");
        return "FAIL";
    }

    string newpref = $"{prefix}{direction}";
    Stack<(string, string)> toTry = new();
    toTry.Push((direction, newpref));
    toTry.Push((changeDirection(direction, 1), newpref));
    toTry.Push((changeDirection(direction, 2), newpref));
    toTry.Push((changeDirection(direction, 3), newpref));

    long total = long.MaxValue;
    string bestPrefix = "FAIL";
    while (toTry.Count > 0)
    {
        // seen, now, is effectively the present path.
        HashSet<(int, int)> newseen = new();
        foreach (var p in seen) newseen.Add(p);
        newseen.Add((startrow, startcol));

        (string mydir, string mypref) = toTry.Pop();
        (int dy, int dx) = directionToDiff(mydir);
        string nP = FindPaths(mypref, mydir, startrow + dy, startcol + dx, endrow, endcol, mazeblockers, memo, newseen);
        if (String.Equals(nP, "FAIL"))
        {
            // nothing to do.
            continue;
        }
        else
        {
            // This needs to put in the memo the best path from here, not the
            // total path!
            nP = $"{mydir}{nP}";
            long t = prefixToCount(nP);
            if (t < total)
            {
                total = t;
                bestPrefix = nP;
            }
        }
    }

    //memo.Add((startrow, startcol), bestPrefix);
    return bestPrefix;
}

(int,int) directionToDiff(string direction)
{
    switch (direction)
    {
        case ">":
            return (0, 1);
        case "<":
            return (0, -1);
        case "^":
            return (-1, 0);
        case "v":
            return (1, 0);
        default: 
            throw new ArgumentException("invalid direction.");
    }
}

string changeDirection(string direction, int n)
{
    if (n == 0) return direction;
    switch (direction)
    {
        case ">":
            return changeDirection("^", n - 1);
        case "<":
            return changeDirection("v", n - 1);
        case "^":
            return changeDirection("<", n - 1);
        case "v":
            return changeDirection(">", n - 1);
        default:
            throw new ArgumentException("arg");
    }
}

long prefixToCount(string s)
{
    long tot = 0;
    for (int i = 1; i < s.Length; i++)
    {
        string sub = s.Substring(i - 1, 2);
        switch (sub)
        {

            case "^^":
            case ">>":
            case "<<":
            case "vv":
                tot += 1;
                break;

            case "<^":
            case "<v":
            case ">^":
            case ">v":
            case "^>":
            case "^<":
            case "v<":
            case "v>":
                tot += 1001;
                break;
            case "><":
            case "^v":
            case "<>":
            case "v^":
                tot += 2001;
                break;
            default:
                throw new ArgumentException("buggered prefix.");
        }
    }
    return tot;
}

long prefixToCost(string s)
{
    long tot = 0;
    switch (s)
    {

        case "^^":
        case ">>":
        case "<<":
        case "vv":
            return 1;

        case "<^":
        case "<v":
        case ">^":
        case ">v":
        case "^>":
        case "^<":
        case "v<":
        case "v>":
            return 1001;
        case "><":
        case "^v":
        case "<>":
        case "v^":
            return 2001;
        default:
            throw new ArgumentException("buggered prefix.");
    }
    
}

