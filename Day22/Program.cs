// See https://aka.ms/new-console-template for more information
using System.Runtime.CompilerServices;

Console.WriteLine("Hello, World!");
int iters = 2000;
var lines = File.ReadAllLines(args[0]);

//Part2Take2(iters, lines);
Part2(iters, lines);
//Part1(iters, lines);

//
// Find a sequence of changes that gives a max bananas sale.
//
// Strategy - go though all secrets, calculating sales vals and diffs.
// Then find all sequences of four differences. For the input that is
// about 40000. Then go through each, calculating the price you'll get
// for each secret, and that's the value.
//
void Part2Take2(int iters, string[] lines)
{
    int[][] diffs = new int[lines.Length][];
    int[][] saleval = new int[lines.Length][];
    int total = 0;
    for(int r = 0; r < lines.Length; r++)
    {
        var line = lines[r];
        diffs[r] = new int[iters];
        saleval[r] = new int[iters];
        diffs[r][0] = -1;
        saleval[r][0] = -1;
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
    }

    HashSet<( int, int, int, int)> foursomes = new();
    (int, int, int, int) firstfour = (-1, -1, -1, -1);
    for (int i = 3; i < iters; i++)
    {
        firstfour = (diffs[0][i - 3], diffs[0][i - 2], diffs[0][i - 1], diffs[0][i]);
        foursomes.Add(firstfour);
    }

    Console.WriteLine($"{foursomes.Count} unique fours.");
    total = 0;
    foreach (var f in foursomes)
    {
        if (f == (-2, 1, -1, 3))
            Console.WriteLine("whoop");
        int t = 0;
        for (int r = 0; r < lines.Length; r++)
        {
            int x = 0;
            for (int i = 3; i < iters; i++)
            {
                if ((diffs[r][i - 3], diffs[r][i - 2], diffs[r][i - 1], diffs[r][i]) == f)
                {
                    //    if (saleval[r][i] > x) 
                    //        x = saleval[r][i];
                    x = saleval[r][i];
                    // once we see the diff, we move to the next seller.
                    // that's what the instructions say, anyway.
                    break;
                }
            }
            t += x;
        }
        if (t > total) 
            total = t;
        Console.WriteLine($"{f} {t} {total}");
    }
    Console.WriteLine(total);
}

void Part2(int iters, string[] lines)
{
    int[][] diffs = new int[lines.Length][];
    int[][] saleval = new int[lines.Length][];
    int total = 0;
    for (int r = 0; r < lines.Length; r++)
    {
        var line = lines[r];
        long secret = long.Parse(line);
        diffs[r] = new int[iters+1];
        saleval[r] = new int[iters + 1];
        diffs[r][0] = 0;
        saleval[r][0] = (int)(secret % 10); // FFS - setting this to -1 was never going to work.
        long lastsecret = 0;
        for (int i = 1; i < iters+1; i++)
        {
            lastsecret = secret;
            secret = SecretSteps(secret);
            saleval[r][i] = (int)(secret % 10);
            diffs[r][i] = i > 0 ? saleval[r][i] - saleval[r][i - 1] : 0;
            //Console.WriteLine($"iter {i} -> {secret}");
        }
    }

    HashSet<(int, int, int, int)> foursomes = new();
    for (int r = 0; r < lines.Length; r++)
    {
        for (int i = 4; i < iters+1; i++)
        {
            (int, int, int, int) four = (diffs[r][i - 3], diffs[r][i - 2], diffs[r][i - 1], diffs[r][i]);
            foursomes.Add(four);
        }
    }
    Console.WriteLine($"{foursomes.Count} unique fours.");
    total = 0;
    foreach (var f in foursomes)
    {
        if (f == (-2, 1, -1, 3))
            Console.WriteLine("whoop");
        int t = 0;
        for (int r = 0; r < lines.Length; r++)
        {
            int x = 0;
            for (int i = 4; i < iters+1; i++)
            {
                if ((diffs[r][i - 3], diffs[r][i - 2], diffs[r][i - 1], diffs[r][i]) == f)
                {
                    //    if (saleval[r][i] > x) 
                    //        x = saleval[r][i];
                    x = saleval[r][i];
                    // once we see the diff, we move to the next seller.
                    // that's what the instructions say, anyway.
                    break;
                }
            }
            t += x;
        }
        if (t > total)
            total = t;
        Console.WriteLine($"{f} {t} {total}");
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

