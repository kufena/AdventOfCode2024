// See https://aka.ms/new-console-template for more information
using System.Runtime.CompilerServices;

Console.WriteLine("Hello, World!");
int iters = 2000;
var lines = File.ReadAllLines(args[0]);

Part2(iters, lines);
//Part1(iters, lines);

//
// Find a sequence of changes that gives a max bananas sale.
//
void Part2(int iters, string[] lines)
{
    int[][] diffs = new int[lines.Length][];
    int[][] saleval = new int[lines.Length][];
    long total = 0;
    for(int r = 0; r < lines.Length; r++)
    {
        var line = lines[r];
        diffs[r] = new int[iters];
        saleval[r] = new int[iters];
        long lastsecret = 0;
        long secret = long.Parse(line);
        for (int i = 0; i < iters; i++)
        {
            lastsecret = secret;
            secret = SecretSteps(secret);
            saleval[r][i] = (int)(secret % 10);
            diffs[r][i] = i > 0 ? saleval[r][i] - saleval[r][i - 1] : 0;
            //Console.WriteLine($"iter {i} -> {secret}");
        }
        total += secret;
    }

    Console.WriteLine(total);
}

// Straightforward apply the rules, get the numbers,
// add them up.
void Part1(int iters, string[] lines)
{
    long total = 0;
    foreach (var line in lines)
    {
        long secret = long.Parse(line);
        for (int i = 0; i < iters; i++)
        {
            secret = SecretSteps(secret);

            //Console.WriteLine($"iter {i} -> {secret}");
        }
        total += secret;
    }

    Console.WriteLine(total);
}


long mix(long a, long b)
{
    long c = a ^ b;
    return c;
}

long prune(long a)
{
    return a % 16777216;
}

long SecretSteps(long secret)
{
    // step 1
    secret = prune(mix(secret * 64, secret));
    // step 2
    secret = prune(mix(secret / 32, secret));
    // step 3
    secret = prune(mix(secret * 2048, secret));
    return secret;
}

