// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
var lines = File.ReadAllLines(args[0]);
int i = 0;
List<(int, int, int, int, int)> Keys = new();
List<(int, int, int, int, int)> Locks = new();
while (true)
{
    if (i >= lines.Length)
        break;

    int[] vals = new int[5] { -1, -1, -1, -1, -1 };
    if (lines[i] == "#####") // it's a lock
    {
        for (int k = 0; k < 7; k++)
        {
            for (int l = 0; l < 5; l++)
            {
                if (lines[i + k][l] == '.' && vals[l] < 0)
                    vals[l] = k;
            }
        }
        Locks.Add((vals[0]-1, vals[1]-1, vals[2]-1, vals[3]-1, vals[4]-1));
    }
    else // it's a key
    {
        for (int k = 6; k >= 0; k--)
        {
            for (int l = 0; l < 5; l++)
            {
                if (lines[i + k][l] == '.' && vals[l] < 0)
                    vals[l] = k;
            }
        }
        Keys.Add((5-vals[0], 5-vals[1], 5-vals[2], 5-vals[3], 5-vals[4]));
    }

    // skip blank line.
    i += 8;
}

foreach (var l in Keys) Console.WriteLine($" Key = {l}");
foreach (var l in Locks) Console.WriteLine($"Lock = {l}");

Console.WriteLine("");

int total = 0;
foreach (var k in Keys)
{
    foreach (var l in Locks)
    {
        if (k.Item1 + l.Item1 < 6 &&
            k.Item2 + l.Item2 < 6 &&
            k.Item3 + l.Item3 < 6 &&
            k.Item4 + l.Item4 < 6 &&
            k.Item5 + l.Item5 < 6)
        {
            total++;
            Console.WriteLine($"{k} and {l} match!");
        }
    }
}
Console.WriteLine($"{total}");