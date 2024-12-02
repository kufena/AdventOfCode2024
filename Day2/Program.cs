// See https://aka.ms/new-console-template for more information
using System.Security.Cryptography;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

int fail = 0;
int safe = 0;

foreach (var line in lines)
{
    var imms = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    long[] levels = new long[imms.Length];
    for (int i = 0; i < imms.Length; i++)
    {
        levels[i] = long.Parse(imms[i]);
    }
    if (OneLineCall(levels)) safe++;
    else
    {
        bool ok = false;
        // faile properly - do part 2 of excluding one by one.
        for (int x = 0; x < levels.Length; x++)
        {
            int c = 0;
            long[] newlevels = new long[levels.Length-1];
            for (int y = 0; y < levels.Length; y++)
            {
                if (x != y)
                {
                    newlevels[c] = levels[y];
                    c++;
                }
            }
            if (OneLineCall(newlevels))
            {
                ok = true;
                break;
            }
        }
        if (ok) safe++; else fail++;
    }
}

Console.WriteLine($"fail = {fail} safe = {safe}");

static bool OneLineCall(long[] levels)
{

    long[] diffs = new long[levels.Length - 1];
    int dircount = 0;
    for (int i = 1; i < levels.Length; i++)
    {
        if (levels[i] > levels[i - 1]) dircount += 1;
        if (levels[i] < levels[i - 1]) dircount += -1;
        diffs[i - 1] = Math.Abs(levels[i] - levels[i - 1]);
    }
    if (dircount == diffs.Length || dircount == -(diffs.Length))
    {
        bool nope = false;
        for (int i = 0; i < diffs.Length; i++)
        {
            if (!(diffs[i] > 0 && diffs[i] < 4))
            {
                nope = true; break;
            }
        }
        return !nope;
    }
    else
        return false;
}