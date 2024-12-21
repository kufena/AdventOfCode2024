using System.Security.Cryptography;
using System.Xml;
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
int maxcheat = 2;
int minsaving = 50;

HashSet<(int, int)> nodes = new();
for (int i = 0; i < rows; i++) {
    for(int j = 0; j < cols; j++) {
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

List<(int, int)> adjacencies = new List<(int, int)>() { (0, 1), (0, -1), (1, 0), (-1, 0) };

Dictionary<(int, int), HashSet<(int, int)>> edges = new();
BuildEdges(nodes, edges);

(int initialSP, List<(int,int)> actualPath) = ShortestPath(nodes, edges, startrow, startcol, endrow, endcol);
Console.WriteLine($"Shortest path is {initialSP} from {nodes.Count} nodes");

Dictionary<int, int> savingCounts = new();
HashSet<(int, int, int, int)> uniqueCheats = new();

for (int i = 1; i < rows-1; i++)
{
    for (int j = 1; j < cols-1; j++)
    {
        //if (i == startrow && j == startcol) continue;
        //if (i == endrow && j == endcol) continue;

        // should give us cheats of lenght 2 to maxcheat.
        for (int count = 1; count < maxcheat; count++)
        {
            //Console.WriteLine($"{(i, j)} count {count} so far {uniqueCheats.Count}");
            HashSet<List<(int, int)>> AllCheats = GenerateCheats(rows, cols, i, j, count);
            foreach (var cheat in AllCheats)
            {
                (int cheatsrow, int cheatscol) = cheat.First();
                (int cheaterow, int cheatecol) = cheat.Last();
                if (!uniqueCheats.Contains((cheatsrow, cheatscol, cheaterow, cheatecol))) 
                {
                    var cheatpath = CheckCheat3(cheat, rows, cols, nodes, startrow, endrow, startcol, endcol);
                    (int len, List<(int, int)> path) = cheatpath;
                    if (len < initialSP)
                    {
                        // check all steps used and in order.
                        if (path.Contains(cheat.First()))
                        {
                            int ind = path.IndexOf(cheat.First());
                            int k = 1;
                            bool ok = true;
                            while (k < cheat.Count)
                            {
                                int pop = path.IndexOf(cheat[k]);
                                if (pop != ind + 1)
                                {
                                    ok = false;
                                    break;
                                }
                                ind = pop;
                                k++;
                            }

                            if (ok)
                            {
                                int saving = initialSP - len;
                                if (savingCounts.ContainsKey(saving))
                                    savingCounts[saving] += 1;
                                else
                                    savingCounts.Add(saving, 1);
                                uniqueCheats.Add((cheatsrow, cheatscol, cheaterow, cheatecol));
                            }
                        }
                    }
                }
            }
        }
    }
}

HashSet<List<(int, int)>> GenerateCheats(int rows, int cols, int i, int j, int v)
{
    if (v == 0)
    {
        if (lines[i][j] != '#')
            return new HashSet<List<(int, int)>>() { new List<(int, int)>() { (i, j) } };
        else
            return new HashSet<List<(int, int)>>() { };
    }

    HashSet<List<(int, int)>> gcresult = new HashSet<List<(int, int)>>();
    foreach ((int dr, int dc) in adjacencies)
    {
        int newr = i + dr;
        int newc = j + dc;
        if (newr < 0 || newr >= rows || newc < 0 || newc >= cols)
        {
            continue;
        }
        var rec = GenerateCheats(rows, cols, newr, newc, v - 1);
        foreach (var list in rec)
        {
            if (!list.Contains((i, j)))
            {
                var nlist = list.Prepend((i,j)).ToList();
                gcresult.Add(nlist);
            }
        }
    }
    return gcresult;
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
(int, List<(int,int)>) CheckCheat2(int r1, int c1, int r2, int c2, int rows, int cols, HashSet<(int, int)> nodes, Dictionary<(int, int), HashSet<(int, int)>> edges, string[] lines, int startrow, int endrow, int startcol, int endcol)
{
    //if (r1 == 3 && r2 == 4 && c1 == 0 && c2 == 0)
    //    Console.WriteLine("");
    if (r1 >= 0 && r1 < rows && r2 >= 0 && r2 < rows && c1 >= 0 && c1 < cols && c2 >= 0 && c2 < cols)
    {
        // This just ensures there is a potential advantage.
        if (lines[r1][c1] == '#' && lines[r2][c2] != '#')
        {
            HashSet<(int, int)> rpnodes = new ();
            foreach (var p in nodes) rpnodes.Add(p);
            rpnodes.Add((r1, c1));
            rpnodes.Add((r2, c2));
            var rpedges = new Dictionary<(int,int), HashSet<(int, int)>>();
            BuildEdges(rpnodes, rpedges);

            var rppath = ShortestPath(rpnodes, rpedges, startrow, startcol, endrow, endcol);
            return rppath;
        }
    }

    return (int.MaxValue, new List<(int, int)>());
}

// For cheats of arbitrary length.
(int, List<(int, int)>) CheckCheat3(List<(int,int)> cheat, int rows, int cols, HashSet<(int, int)> nodes, int startrow, int endrow, int startcol, int endcol)
{
    // we assume all are within bounds of the board, and that
    // the cheat ends with a non-wall.
    HashSet<(int, int)> rpnodes = new();
    foreach (var p in nodes) rpnodes.Add(p);
    foreach ((var crow, var ccol) in cheat)
        rpnodes.Add((crow, ccol));
    var rpedges = new Dictionary<(int, int), HashSet<(int, int)>>();
    BuildEdges(rpnodes, rpedges);

    var rppath = ShortestPath(rpnodes, rpedges, startrow, startcol, endrow, endcol);
    return rppath;
}

(int,List<(int,int)>) ShortestPath(HashSet<(int, int)> nodes, Dictionary<(int, int), HashSet<(int, int)>> edges, int startrow, int startcol, int endrow, int endcol)
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

    (int,int) prv = prev[target];
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

static void CheckNewPath(int initialSP, Dictionary<int, int> savingCounts, (int, List<(int,int)>) r1path, int r1, int c1, int r2, int c2, HashSet<(int, int, int, int)> uniqueCheats)
{
    (int r1pathlen, List<(int, int)> r1actualPath) = r1path;
    int cheat1count = r1actualPath.Select(x => x).Where(x => (x == (r1, c1))).Count();
    int cheat2count = r1actualPath.Select(x => x).Where(x => (x == (r2, c2))).Count();
    int r1index = r1actualPath.IndexOf((r1, c1));
    int r2index = r1actualPath.IndexOf((r2, c2));

    if (r1pathlen < initialSP && cheat1count == 1 && cheat2count == 1 && (r2index == r1index+1))
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