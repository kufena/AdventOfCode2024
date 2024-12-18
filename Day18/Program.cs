// See https://aka.ms/new-console-template for more information
using System.ComponentModel;
using System.Security.Cryptography;
using Utilities;

Console.WriteLine("Hello, World!");

int gridrows = 71;
int gridcols = 71;
int steps = 1024;

var lines = File.ReadAllLines(args[0]);

HashSet<(int, int)> nodes = new();
for (int i = 0; i < gridrows; i++)
{
    for (int j = 0; j < gridcols; j++)
    {
        nodes.Add((i, j));
    }
}

for (int k = 0; k < steps; k++)
{
    var tmp = lines[k].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray();
    nodes.Remove((tmp[1],tmp[0]));
}

for (int l = steps; l < lines.Length; l++)
{
    var tmp = lines[l].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToArray();
    nodes.Remove((tmp[1], tmp[0]));

    Dictionary<(int, int), List<(int, int)>> edges = new();
    foreach (var node in nodes)
    {
        (int y, int x) = node;
        List<(int, int)> nedges = new();
        if (nodes.Contains((y - 1, x))) nedges.Add((y - 1, x));
        if (nodes.Contains((y + 1, x))) nedges.Add((y + 1, x));
        if (nodes.Contains((y, x - 1))) nedges.Add((y, x - 1));
        if (nodes.Contains((y, x + 1))) nedges.Add((y, x + 1));
        edges.Add(node, nedges);
    }

    int tr = ShortestPath(nodes, edges, 0, 0, gridrows - 1, gridcols - 1);
    if (tr == 0 || tr == int.MaxValue)
    {
        Console.WriteLine($"{tmp[0]} {tmp[1]}");
        break;
    }
}
int ShortestPath(HashSet<(int, int)> nodes, Dictionary<(int,int),List<(int,int)>> edges, int startrow, int startcol, int endrow, int endcol)
{
    UpdatablePriorityQueue upq = new();
    Dictionary<(int,int), int> distance = new();
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
        List<(int, int)> nedges = edges[u];
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

    Console.WriteLine($"Shortest appears to be {distance[target]}");
    return distance[target];

    //Node prv = prev[target];
    //List<(int, int)> steps = new List<(int, int)>() { (endrow, endcol) };
    //while (prv != startNode)
    //{
    //    steps.Add((prv.row, prv.col));
    //    prv = prev[prv];
    //}
    //steps.Reverse();
    //Console.WriteLine($"What now?");
}
