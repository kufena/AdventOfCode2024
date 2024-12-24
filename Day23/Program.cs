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

//var nodearr = nodes.ToArray();
//int n = 0;
//for (int i = 0; i < nodearr.Length - 2; i++)
//{
//    for (int j = 1; j < nodearr.Length - 1; j++)
//        for (int k = 2; k < nodearr.Length; k++)
//        {
//            string[] dim = new string[3] { nodearr[i], nodearr[j], nodearr[k] };
//            Array.Sort(dim); // these should be unique triples.
//            if (edges.Contains((dim[0], dim[1])) && edges.Contains((dim[1], dim[2])) && edges.Contains((dim[0], dim[2])))
//            {
//                if (!triples.Contains((dim[0], dim[1], dim[2])))
//                {
//                    triples.Add((dim[0], dim[1], dim[2]));
//                    if (dim[0].StartsWith("t") || dim[1].StartsWith("t") || dim[2].StartsWith("t"))
//                        count++;
//                }
//            }
//            n++;
//            if (n % 1000 == 0) Console.WriteLine(n);
//        }
//}
Console.WriteLine($"triples ={triples.Count}");
foreach (var t in triples)
{
    if (t.Item1.StartsWith("t") || t.Item2.StartsWith("t") || t.Item3.StartsWith("t"))
        count++;
    Console.WriteLine(t);
}
Console.WriteLine(count);