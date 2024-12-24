// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

(string, string)[] pairs = new (string,string)[lines.Length*2];
HashSet<string> nodes = new();
for(int i = 0; i < lines.Length*2; i += 2)
{
    var line = lines[i/2];
    var dedges = line.Split('-');
    Array.Sort(dedges);
    pairs[i] = (dedges[0], dedges[1]);
    pairs[i+1] = (dedges[1], dedges[0]);
    nodes.Add(dedges[0]);
    nodes.Add(dedges[1]);
}
Array.Sort(pairs);
var edges = pairs.ToList();

Console.WriteLine($"{pairs.Length} pairs");
Console.WriteLine($"{nodes.Count} nodes");
int count = 0;
HashSet<(string, string, string)> triples = new();

foreach (var p in edges)
{
    var medges = edges.Where(x => p.Item2 == x.Item1 || p.Item2 == x.Item2).ToList();
    foreach (var m in medges)
    {
        if (m == p || (m.Item1 == p.Item2 && m.Item2 == p.Item1))
            continue;
        if (p.Item2 == m.Item1)
        {
            string[] mid = new string[] { p.Item1, p.Item2, m.Item2 };
            Array.Sort(mid);
            if (edges.Contains((mid[0], mid[1])) && edges.Contains((mid[0], mid[2])) && edges.Contains((mid[1], mid[2])))
                triples.Add((mid[0], mid[1], mid[2]));
        }
        else
        {
            string[] mid = new string[] { p.Item1, p.Item2, m.Item1 };
            Array.Sort(mid);
            if (edges.Contains((mid[0],mid[1])) && edges.Contains((mid[0],mid[2])) && edges.Contains((mid[1],mid[2])))
                triples.Add((mid[0], mid[1], mid[2]));
        }
    }
}

Console.WriteLine($"triples ={triples.Count}");
//foreach (var t in triples)
//{
//    if (t.Item1.StartsWith("t") || t.Item2.StartsWith("t") || t.Item3.StartsWith("t"))
//        count++;
//    Console.WriteLine(t);
//}
Console.WriteLine(count);

List<HashSet<string>> all = new();
int jj = 0;
foreach (var t in triples)
{
    jj++;
    Console.WriteLine($"Triple {t} num {jj}");
    HashSet<string> bigset = new();
    bigset.Add(t.Item1);
    bigset.Add(t.Item2);
    bigset.Add(t.Item3);

    bool change = true;
    while (change)
    {
        HashSet<string> newset = new() ;
        bool first = true;
        foreach (var x in bigset)
        {
            HashSet<string> nedgeupset = new();
            var nedgeup = edges.
                            Where(p => p.Item1 == x || p.Item2 == x).
                            ToList();
            // we need an intersection somewhere.
            foreach (var nod in nedgeup)
            {
                nedgeupset.Add(nod.Item1);
                nedgeupset.Add(nod.Item2);
            }
            if (first)
            {
                first = false;
                newset = nedgeupset;
            }
            else
            {
                newset = newset.Intersect(nedgeupset).ToHashSet();
            }
        }

        if (bigset.Count > newset.Count)
        {
            bigset = newset;
            break; // this should not happen?
        }
        change = (bigset.Count != newset.Count);
        if (jj == 11)
            Console.WriteLine($"{bigset.Count} vs {newset.Count} round and round and round.");
        bigset = newset;
    }
    all.Add(bigset);
}

int size = 0;
HashSet<string> theone = new();
foreach (var bs in all)
{
    if (bs.Count > size)
    {
        size = bs.Count;
        theone = bs;
    }
}

Console.WriteLine($"{size}");
if (theone != null)
{
    string[] sz = new string[theone.Count];
    int i = 0;
    foreach (var s in theone)
    {
        sz[i] = s;
        i++;
    }
    Array.Sort(sz);
    for (i = 0; i < sz.Length; i++)
        Console.Write($"{sz[i]},");
    Console.WriteLine($"");
}