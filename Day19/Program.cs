// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

var spl1 = lines[0].Split(',', StringSplitOptions.RemoveEmptyEntries);
string[] towels = new string[spl1.Length];
for (int i = 0; i < spl1.Length; i++) towels[i] = spl1[i].Trim();

//Part1(lines, towels);
Part2(lines, towels);

void Part2(string[] lines, string[] towels)
{
    long good = 0;
    for (int k = 2; k < lines.Length; k++)
    {
        long matches = MatchEnumerate(towels, lines[k]);
        Console.WriteLine($"GOOD {matches} {lines[k]}");
        good += matches;

    }
    Console.WriteLine($"Good = {good}");
}

long MatchEnumerate(string[] towels, string pattern)
{
    int patLen = pattern.Length;
    // If we've eaten an amount of the string, it doesn't matter how much is left - do the rest.
    // So if we match in several ways, it doesn't matter.
    Dictionary<int,long> sofar = new();
    sofar.Add(0,1);
    long result = 0;
    while (sofar.Count > 0)
    {
        Dictionary<int,long> newsofar = new();

        foreach ((int eaten, long count) in sofar)
        {
            foreach (var towel in towels)
            {
                bool match = true;
                for (int i = 0; i < towel.Length; i++)
                {
                    if (eaten + i >= patLen)
                    {
                        match = false;
                        break;
                    }
                    if (pattern[eaten + i] != towel[i])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    int left = eaten + towel.Length;
                    if (left == pattern.Length)
                    {
                        // we're only about matching, no enumeration etc, so just return now.
                        result += count;
                    }
                    else
                    {
                        int neweaten = eaten + towel.Length;
                        if (newsofar.ContainsKey(neweaten))
                            newsofar[neweaten] += count;
                        else
                            newsofar.Add(neweaten, count);
                    }
                }
            }
        }
        sofar = newsofar;
    }
    return result;
}

bool Match(string[] towels, string pattern)
{
    int patLen = pattern.Length;
    // If we've eaten an amount of the string, it doesn't matter how much is left - do the rest.
    // So if we match in several ways, it doesn't matter.
    HashSet<int> sofar = new();
    sofar.Add(0);

    while (sofar.Count > 0)
    {
        HashSet<int> newsofar = new();

        foreach (int eaten in sofar)
        {
            foreach (var towel in towels)
            {
                if (newsofar.Contains(eaten + towel.Length))
                    continue;

                bool match = true;
                for (int i = 0; i < towel.Length; i++)
                {
                    if (eaten + i >= patLen)
                    {
                        match = false;
                        break;
                    }
                    if (pattern[eaten + i] != towel[i])
                    {
                        match = false;
                        break;
                    }
                }
                if (match)
                {
                    int left = eaten + towel.Length;
                    if (left == pattern.Length)
                    {
                        // we're only about matching, no enumeration etc, so just return now.
                        return true;
                    }

                    newsofar.Add(eaten + towel.Length);
                }
            }
        }
        sofar = newsofar;
    }
    return false;
}

void Part1(string[] lines, string[] towels)
{
    int good = 0;
    for (int k = 2; k < lines.Length; k++)
    {
        if (Match(towels, lines[k].Trim()))
        {
            good++;
            Console.WriteLine($"GOOD {lines[k]}");
        }
        else
        {
            Console.WriteLine($"BADD {lines[k]}");
        }
    }
    Console.WriteLine($"Good = {good}");
}