// See https://aka.ms/new-console-template for more information
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);
Console.WriteLine(lines.Length);

//Part1(lines);
Part2(lines);

static void Part2(string[] lines)
{
    Regex matcher = new Regex(@"(mul\([0-9]+\,[0-9]+\))|(don\'t\(\))|(do\(\))", RegexOptions.None);


    long total = 0;
    bool incl = true;

    foreach (var line in lines)
    {

        Console.WriteLine(line);
        foreach (var x in matcher.EnumerateMatches(new ReadOnlySpan<char>(line.ToArray<char>())))
        {
            Console.WriteLine($"{x.Index} to {x.Length}");
            string sub = line.Substring(x.Index, x.Length);
            Console.WriteLine(sub);
            if (sub.StartsWith("don't"))
            {
                incl = false;
            }
            else if (sub.StartsWith("do()")) incl = true;
            else
            {
                if (incl)
                {
                    var subsplits = sub.Split(new char[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var t in subsplits) Console.WriteLine(t);
                    long l1 = long.Parse(subsplits[1]);
                    long l2 = long.Parse(subsplits[2]);
                    total += (l1 * l2);
                    Console.WriteLine($"{l1} * {l2} = {l1 * l2} {total}");
                }
            }
        }

    }
}

static void Part1(string[] lines)
{

    Regex matcher = new Regex(@"mul\([0-9]+\,[0-9]+\)", RegexOptions.None);

    long total = 0;
    foreach (var line in lines)
    {

        Console.WriteLine(line);
        foreach (var x in matcher.EnumerateMatches(new ReadOnlySpan<char>(line.ToArray<char>())))
        {
            Console.WriteLine($"{x.Index} to {x.Length}");
            string sub = line.Substring(x.Index, x.Length);
            var subsplits = sub.Split(new char[] { '(', ')', ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var t in subsplits) Console.WriteLine(t);
            long l1 = long.Parse(subsplits[1]);
            long l2 = long.Parse(subsplits[2]);
            total += (l1 * l2);
            Console.WriteLine($"{l1} * {l2} = {l1 * l2} {total}");
        }

    }
}

/**
static long consumeNum(string s, int index, out int newindex) 
{

    long val = 0;
    newindex = index;
    while (Char.IsDigit(s[newindex]))
    {
        val = (val * 10) + s[newindex];
    }
    return val;
}

static bool consumeChar(char c, string s, int index, out int newindex) 
{
    newindex = index;
    if (s[newindex] != c) return false;
    newindex++;
    return true;
}
*/