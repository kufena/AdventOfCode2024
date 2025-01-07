
using System.Security.Cryptography;
using Utilities;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);
int rows = lines.Length;
int cols = lines[0].Length;

int startrow = 0;
int startcol = 0;
int endrow = 0;
int endcol = 0;

HashSet<(int, int)> nodes = new();
for (int i = 0; i < rows; i++)
{
    for (int j = 0; j < cols; j++)
    {
        if (lines[i][j] == '.')
        {
            nodes.Add((i, j));
        }
        if (lines[i][j] == 'S')
        {
            nodes.Add((i, j));
            startrow = i;
            startcol = j;
        }
        if (lines[i][j] == 'E')
        {
            nodes.Add((i, j));
            endrow = i;
            endcol = j;
        }
    }
}

Dictionary<(int, int), HashSet<(int, int)>> edges = new();
BuildEdges(nodes, edges);

(int initialSP, List<(int, int)> actualPath) = ShortestPath(nodes, edges, startrow, startcol, endrow, endcol);
Console.WriteLine($"Shortest path is {initialSP} from {nodes.Count} nodes");

Dictionary<int, int> savingCounts = new();
HashSet<(int, int, int, int)> uniqueCheats = new();

for (int i = 0; i < rows; i++)
{
    for (int j = 0; j < cols; j++)
    {
        //if (i == startrow && j == startcol) continue;
        //if (i == endrow && j == endcol) continue;

        // BEWARE DUPLICATE CHEATS.
        var r1path = CheckCheat2(i, j, i + 1, j, rows, cols, nodes, edges, lines, startrow, endrow, startcol, endcol);
        var r2path = CheckCheat2(i, j, i - 1, j, rows, cols, nodes, edges, lines, startrow, endrow, startcol, endcol);
        var c1path = CheckCheat2(i, j, i, j + 1, rows, cols, nodes, edges, lines, startrow, endrow, startcol, endcol);
        var c2path = CheckCheat2(i, j, i, j - 1, rows, cols, nodes, edges, lines, startrow, endrow, startcol, endcol);
        CheckNewPath(initialSP, savingCounts, r1path, i, j, i + 1, j, uniqueCheats);
        CheckNewPath(initialSP, savingCounts, r2path, i, j, i - 1, j, uniqueCheats);
        CheckNewPath(initialSP, savingCounts, c1path, i, j, i, j + 1, uniqueCheats);
        CheckNewPath(initialSP, savingCounts, c2path, i, j, i, j - 1, uniqueCheats);
    }
}

int result = 0;
foreach ((int k1, int v1) in savingCounts)
{
    Console.WriteLine($"There are {v1} cheats savings of {k1} picoseconds");
    if (k1 >= 100) result += v1;
}
Console.WriteLine($"{uniqueCheats.Count} unique cheats.");
Console.WriteLine($"{result} cheats save at least 100 picoseconds");

//
// This is a simpler version that simple removes a single # at a time, effectively.
// It does this by making the second point have to be a '.' or 'S' or 'E'.
(int, List<(int, int)>) CheckCheat2(int r1, int c1, int r2, int c2, int rows, int cols, HashSet<(int, int)> nodes, Dictionary<(int, int), HashSet<(int, int)>> edges, string[] lines, int startrow, int endrow, int startcol, int endcol)
{
    //if (r1 == 3 && r2 == 4 && c1 == 0 && c2 == 0)
    //    Console.WriteLine("");
    if (r1 >= 0 && r1 < rows && r2 >= 0 && r2 < rows && c1 >= 0 && c1 < cols && c2 >= 0 && c2 < cols)
    {
        // This just ensures there is a potential advantage.
        if (lines[r1][c1] == '#' && lines[r2][c2] != '#')
        {
            HashSet<(int, int)> rpnodes = new();
            foreach (var p in nodes) rpnodes.Add(p);
            rpnodes.Add((r1, c1));
            rpnodes.Add((r2, c2));
            var rpedges = new Dictionary<(int, int), HashSet<(int, int)>>();
            BuildEdges(rpnodes, rpedges);

            var rppath = ShortestPath(rpnodes, rpedges, startrow, startcol, endrow, endcol);
            return rppath;
        }
    }

    return (int.MaxValue, new List<(int, int)>());
}

(int, List<(int, int)>) CheckCheat(int r1, int c1, int r2, int c2, int rows, int cols, HashSet<(int, int)> nodes, Dictionary<(int, int), HashSet<(int, int)>> edges, string[] lines, int startrow, int endrow, int startcol, int endcol)
{
    if (r1 >= 0 && r1 < rows && r2 >= 0 && r2 < rows && c1 >= 0 && c1 < cols && c2 >= 0 && c2 < cols)
    {
        // This just ensures there is a potential advantage.
        if (lines[r1][c1] == '#' || lines[r2][c2] == '#')
        {
            HashSet<(int, int)> rpnodes = new();
            foreach (var p in nodes) rpnodes.Add(p);
            var rpedges = new Dictionary<(int, int), HashSet<(int, int)>>();
            foreach ((var node, var list) in edges)
            {
                var cpedges = new HashSet<(int, int)>();
                foreach (var edgep in list) cpedges.Add(edgep);
                rpedges.Add(node, cpedges);
            }
            // each cheat is directional so r1 to r2 edge needs adding
            // and any links in and out of r1 and r2.
            if (lines[r1][c1] == '#')
            {
                rpnodes.Add((r1, c1));
                // Add edges into (r1,c1)
                if (nodes.Contains((r1 - 1, c1))) rpedges[(r1 - 1, c1)].Add((r1, c1));
                if (nodes.Contains((r1 + 1, c1))) rpedges[(r1 + 1, c1)].Add((r1, c1));
                if (nodes.Contains((r1, c1 - 1))) rpedges[(r1, c1 - 1)].Add((r1, c1));
                if (nodes.Contains((r1, c1 + 1))) rpedges[(r1, c1 + 1)].Add((r1, c1));
                rpedges.Add((r1, c1), new HashSet<(int, int)>() { (r2, c2) });
            }
            else
            {
                // remove edges from (r1,c1) to other nodes except new (r2,c2)
                rpedges.Remove((r1, c1));
                rpedges.Add((r1, c1), new HashSet<(int, int)>() { (r2, c2) });
            }

            if (lines[r2][c2] == '#')
            {
                rpnodes.Add((r2, c2));
                // there should already be an edge to this node.
                var newedges = new HashSet<(int, int)>();
                rpedges.Add((r2, c2), newedges);
                // add edges from r2,c2 to neighbours except r1,c1
                if (nodes.Contains((r2 - 1, c2))) newedges.Add((r2 - 1, c2));
                if (nodes.Contains((r2 + 1, c2))) newedges.Add((r2 + 1, c2));
                if (nodes.Contains((r2, c2 - 1))) newedges.Add((r2, c2 - 1));
                if (nodes.Contains((r2, c2 + 1))) newedges.Add((r2, c2 + 1));
                if (newedges.Contains((r1, c1))) newedges.Remove((r1, c1));
            }
            else
            {
                // remove edges from other nodes into (r2,c2) except (r1,c1)
                if (nodes.Contains((r2 - 1, c2))) rpedges[(r2 - 1, c2)].Remove((r2, c2));
                if (nodes.Contains((r2 + 1, c2))) rpedges[(r2 + 1, c2)].Remove((r2, c2));
                if (nodes.Contains((r2, c2 - 1))) rpedges[(r2, c2 - 1)].Remove((r2, c2));
                if (nodes.Contains((r2, c2 + 1))) rpedges[(r2, c2 + 1)].Remove((r2, c2));
            }

            var rppath = ShortestPath(rpnodes, rpedges, startrow, startcol, endrow, endcol);
            //if (edges[(3,1)].Contains((2, 1)))
            //    Console.WriteLine("woopsie");
            return rppath;
        }
    }
    //if (edges[(3,1)].Contains((2, 1)))
    //    Console.WriteLine("woopsie");

    return (int.MaxValue, new List<(int, int)>());
}

(int, List<(int, int)>) ShortestPath(HashSet<(int, int)> nodes, Dictionary<(int, int), HashSet<(int, int)>> edges, int startrow, int startcol, int endrow, int endcol)
{
    UpdatablePriorityQueue upq = new();
    Dictionary<(int, int), int> distance = new();
    Dictionary<(int, int), (int, int)> prev = new();

    (int, int) target = (endrow, endcol);

    foreach (var nod in nodes)
    {
        (int nodrow, int nodcol) = nod;
        distance[nod] = int.MaxValue;
        prev[nod] = nod;
        upq.Enqueue((nodrow, nodcol), int.MaxValue);
    }
    var startNode = (startrow, startcol);
    distance[startNode] = 0;
    upq.UpdatePriority((startrow, startcol), 0);

    while (upq.Count() > 0)
    {
        ((int, int) u, int udistance) = upq.Dequeue();
        HashSet<(int, int)> nedges = edges[u];
        foreach (var v in nedges)
        {
            if (v == u) continue;
            int alt = udistance + 1;
            if (alt >= 0 && alt < distance[v])
            {
                if (alt < 0)
                    Console.WriteLine("ohdear");
                (int vrow, int vcol) = v;
                distance[v] = alt;
                prev[v] = u;
                upq.UpdatePriority((vrow, vcol), alt);
            }
        }
    }

    //Console.WriteLine($"Shortest appears to be {distance[target]}");

    (int, int) prv = prev[target];
    List<(int, int)> steps = new List<(int, int)>() { (endrow, endcol) };
    if (distance[target] < int.MaxValue)
    {
        while (prv != startNode)
        {
            steps.Add((prv.Item1, prv.Item2));
            prv = prev[prv];
        }
        steps.Add(startNode);
        steps.Reverse();
    }
    return (distance[target], steps);
}

static void BuildEdges(HashSet<(int, int)> nodes, Dictionary<(int, int), HashSet<(int, int)>> edges)
{
    foreach ((int r, int c) in nodes)
    {
        HashSet<(int, int)> nedges = new();
        edges.Add((r, c), nedges);
        if (nodes.Contains((r - 1, c))) nedges.Add((r - 1, c));
        if (nodes.Contains((r + 1, c))) nedges.Add((r + 1, c));
        if (nodes.Contains((r, c - 1))) nedges.Add((r, c - 1));
        if (nodes.Contains((r, c + 1))) nedges.Add((r, c + 1));
    }
}

static void CheckNewPath(int initialSP, Dictionary<int, int> savingCounts, (int, List<(int, int)>) r1path, int r1, int c1, int r2, int c2, HashSet<(int, int, int, int)> uniqueCheats)
{
    (int r1pathlen, List<(int, int)> r1actualPath) = r1path;
    int cheat1count = r1actualPath.Select(x => x).Where(x => (x == (r1, c1))).Count();
    int cheat2count = r1actualPath.Select(x => x).Where(x => (x == (r2, c2))).Count();
    int r1index = r1actualPath.IndexOf((r1, c1));
    int r2index = r1actualPath.IndexOf((r2, c2));

    if (r1pathlen < initialSP && cheat1count == 1 && cheat2count == 1 && (r2index == r1index + 1))
    {
        int savingr1 = initialSP - r1pathlen;
        if (savingCounts.ContainsKey(savingr1)) savingCounts[savingr1] += 1; else savingCounts.Add(savingr1, 1);
        if (savingr1 == 18) Console.WriteLine($"Found one at {r1pathlen}, saving {savingr1} Cheat is {(r1, c1)} to {(r2, c2)}");
        uniqueCheats.Add((r1, c1, r2, c2));
    }
    //else
    //{
    //    if (r1pathlen < initialSP)
    //        Console.WriteLine($"Found one that uses the cheat {cheat1count} or {cheat2count} times {(r1index,r2index)}");
    //}
}
