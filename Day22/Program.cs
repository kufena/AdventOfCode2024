// See https://aka.ms/new-console-template for more information
using System.Runtime.CompilerServices;

Console.WriteLine("Hello, World!");
int iters = 2000;
var lines = File.ReadAllLines(args[0]);

Part1(iters, lines);

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