// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var file = args[0].Trim();
Console.WriteLine(file);

var lines = File.ReadAllLines(file);
var left = new long[lines.Length];
var right = new long[lines.Length];

Dictionary<long,long> leftcounts = new Dictionary<long,long>();
Dictionary<long,long> rightcounts = new Dictionary<long,long>();

long total = 0;

for (int i = 0; i < lines.Length; i++)
{
    var pair = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    left[i] = long.Parse(pair[0]);
    right[i] = long.Parse(pair[1]);

    if (!leftcounts.ContainsKey(left[i])) leftcounts[left[i]] = 1; else leftcounts[left[i]] += 1;
    if (!rightcounts.ContainsKey(right[i])) rightcounts[right[i]] = 1; else rightcounts[right[i]] += 1;
}

Array.Sort(left);
Array.Sort(right);

for (int i = 0; i < lines.Length; i++)
{
    var dif = Math.Abs(left[i] - right[i]);
    Console.WriteLine(dif);
    total += dif;
}

Console.WriteLine($"Part1 Total = {total}");

total = 0;

foreach (var key in leftcounts.Keys)
{
    if (rightcounts.ContainsKey(key))
    {
        total += rightcounts[key] * leftcounts[key] * key;
    }
}

Console.WriteLine($"Part2 Total = {total}");