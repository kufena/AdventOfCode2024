// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);
var times = int.Parse(args[1]);

Dictionary<string, long> numCounts = new();
var splits = lines[0].Split(" ", StringSplitOptions.RemoveEmptyEntries);
foreach (var l in splits)
{
    if (numCounts.ContainsKey(l))
        numCounts[l]++;
    else 
        numCounts[l] = 1;
}

for (int i = 0; i < times; i++)
{
    Dictionary<string, long> newNumCounts = new();
    foreach (var k in numCounts.Keys)
    {
        long count = numCounts[k];
        if (String.Equals(k, "0"))
        {
            if (newNumCounts.ContainsKey("1"))
                newNumCounts["1"] += count;
            else
                newNumCounts["1"] = count;
        }
        else if (k.Length % 2 == 0) // even digits
        {
            var k1 = k.Substring(0, k.Length / 2);
            var k2 = k.Substring(k.Length / 2);
            var k1_s = $"{long.Parse(k1)}";
            var k2_s = $"{long.Parse(k2)}";
            if (newNumCounts.ContainsKey(k1_s)) { newNumCounts[k1_s] += count; }
            else { newNumCounts[k1_s] = count; }
            if (newNumCounts.ContainsKey(k2_s)) { newNumCounts[k2_s] += count; }
            else { newNumCounts[k2_s] = count; }
        }
        else 
        {
            string newK = $"{long.Parse(k) * 2024}";
            if (newNumCounts.ContainsKey(newK)) newNumCounts[newK] += count;
            else newNumCounts[newK] = count;
        }
    }
    numCounts = newNumCounts;
}


long total = 0;
foreach (var pair in numCounts)
{
    Console.WriteLine($"{pair.Key} appears {pair.Value} times.");
    total += pair.Value;
}
Console.WriteLine($"{total} stones.");