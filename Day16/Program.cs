// See https://aka.ms/new-console-template for more information

using Day16;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

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


//AllPaths(nodes, graphNodes, edges, startrow, startcol, endrow, endcol);
AllPathsDFS(nodes, graphNodes, edges, startrow, startcol, endrow, endcol);

//Thread th = new Thread(() =>
//{
//    HashSet<(int, int)> seen = new();
//    Dictionary<(int, int), string> memo = new();
//    string prefix = "";
//    string path = FindPaths(prefix, ">", startrow, startcol, endrow, endcol, mazeblockers, memo, seen);
//    path = $">{path}";
//    Console.WriteLine($"{path}");
//    Console.WriteLine($"{prefixToCount(path)}");
//}, int.MaxValue);
//th.Start();
//th.Join();

void AllPaths(Dictionary<Node, (int, int)> nodes, Dictionary<(int, int), Node> graphNodes, Dictionary<(int, int), HashSet<(int, int)>> edges, int startrow, int startcol, int endrow, int endcol)
{
    HashSet<string> results = new();
    Queue<List<(int, int)>> toDo = new();
    long total = 322060; // long.MaxValue;
    toDo.Enqueue(new List<(int, int)>() { (startrow, startcol) });
    while (toDo.Count > 0)
    {
        Console.WriteLine($"{toDo.Count} left. {results.Count} results so far.");
        var nextStep = toDo.Dequeue();
        (int nr, int nc) = nextStep.Last();

        if (nr == endrow && nc == endcol)
        {
            results.Add(pathToString(nextStep));
            long val = prefixToCount(pathToString(nextStep));
            if (val < total) total = val;
            continue;
        }

        if (edges.ContainsKey((nr, nc))) // it has edges!
        {
            var toadd = edges[(nr, nc)];
            foreach ((int candr, int candc) in toadd)
            {
                if (!nextStep.Contains((candr, candc))) // no loops
                {
                    List<(int, int)> candlist = new();
                    foreach (var p in nextStep) candlist.Add(p);
                    candlist.Add((candr, candc));
                    if (prefixToCount(pathToString(candlist)) < total) // not going to do better so bail out.
                        toDo.Enqueue(candlist);
                
                }
                //else
                //    Console.WriteLine("Loop found - throw it away.");
            }
        }
    }
    Console.WriteLine($"Found {results.Count} paths.");
    Console.WriteLine($"Shortest is {total}");
}

void AllPathsDFS(Dictionary<Node, (int, int)> nodes, Dictionary<(int, int), Node> graphNodes, Dictionary<(int, int), HashSet<(int, int)>> edges, int startrow, int startcol, int endrow, int endcol)
{
    Dictionary<Node, List<List<Node>> memo = new();

    long total = long.MaxValue;
    Stack<Node> nodesLeft = new();
    Node start = graphNodes[(startrow, startcol)];
    nodesLeft.Push(start);
    while (nodesLeft.Count > 0)
    {
        Node n = nodesLeft.Peek();

        if (n.row == endrow && n.col == endcol)
        {
            List<(int, int)> path = new();
            for (int i = 0; i < nodesLeft.Count; i++)
            {
                path.Add((nodesLeft.ElementAt(i).row, nodesLeft.ElementAt(i).col));
            }
            long t = prefixToCount(pathToString(path));
            Console.WriteLine($"Found a path of length {t}");
            Console.WriteLine(pathToString(path));
            if (t < total) total = t;
            nodesLeft.Pop();
            continue;
        }

        Node to = n.NextEdge();
        while (to != null)
        {
            if (!nodesLeft.Contains(to))
            {
                nodesLeft.Push(to);
                break;
            }
            to = n.NextEdge();
        }

        if (to == null)
        {
            n.Reset();
            nodesLeft.Pop();
        }
    }
    Console.WriteLine($"Lowest cost is {total}");
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
    throw new ArgumentException($"funny pairs {arow} {acol} to {brow} {bcol}");
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

