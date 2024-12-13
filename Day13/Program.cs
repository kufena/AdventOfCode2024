// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

static (long,long) SplitButtonLine(string aButtonLine)
{
    var splits = aButtonLine.Split(new char[] { ' ', ',', ':' }, StringSplitOptions.RemoveEmptyEntries);
    long xdiff = long.Parse(splits[2].Split("+")[1]);
    long ydiff = long.Parse(splits[3].Split("+")[1]);
    return (xdiff, ydiff);
}

var lines = File.ReadAllLines(args[0]);

long bCost = 1;
long aCost = 3;
long totalCost = 0;
long addition = 10000000000000;

for (long i = 0; i < lines.Length; i += 4)
{
    var aButtonLine = lines[i];
    var bButtonLines = lines[i + 1];
    var targetLine = lines[i + 2];

    (long xdiff_A, long ydiff_A) = SplitButtonLine(aButtonLine);
    (long xdiff_B, long ydiff_B) = SplitButtonLine(bButtonLines);

    var tsplits = targetLine.Split(new char[] { '=', ',' }, StringSplitOptions.RemoveEmptyEntries);
    var xTarget = long.Parse(tsplits[1]) + addition;
    var yTarget = long.Parse(tsplits[3]) + addition;

    //Console.WriteLine($"First has targets {xTarget},{yTarget} A=({xdiff_A},{ydiff_A}) B=({xdiff_B},{ydiff_B})");

    //
    // Two linear equations are
    //   xdiff_A m + xdiff_B n = xTarget
    //   ydiff_A m + ydiff_B n = yTarget
    //
    // Solve for m and n, which gives us something to aim for.
    // But these might not be optimal?
    double factor = (double) ((double)xdiff_A / (double)ydiff_A);
    double ybdiff = factor * (double)ydiff_B;
    double yt = factor * (double)yTarget;

    if (double.Equals(yt, (double)xTarget))
        Console.WriteLine("oh shit?");

    double n = ((double)xTarget - yt) / ((double)xdiff_B - ybdiff);
    double m = (xTarget - (xdiff_B * n)) / (double)xdiff_A;

    long round_n = (long)(n + 0.1);
    long round_m = (long)(m + 0.1);

    double cost = (m * aCost) + (n * bCost);

    long xtarg_calc = (round_m * xdiff_A) + (round_n * xdiff_B);
    long ytarg_calc = (round_m * ydiff_A) + (round_n * ydiff_B);

//    if (xtarg_calc == xTarget && ytarg_calc == yTarget)
//        Console.WriteLine("It's A HITIHITHITHITHITHITHITHITHITHITHITHITHITHITHITHITHITHITHIT");
//    else
//        Console.WriteLine("It's a MISMISMISMISMISMISMISMISMISMISMISMISMISMISMISMISMISMISMIS");
    
//    if (Nearlylongeger(n, round_n, 0.0001) && Nearlylongeger(m, round_m, 0.0001) && round_n >= 0 && round_m >= 0)
    if (xtarg_calc == xTarget && ytarg_calc == yTarget)
        totalCost += (round_m * aCost) + (round_n * bCost);

//    Console.WriteLine($"m = {m} and n = {n} cost = {cost} {totalCost}");
}

Console.WriteLine($"Total cost = {totalCost}");

static bool Nearlylongeger(double n, long n_int, double tolerance)
{
    return (Math.Abs((n - (double)n_int)) < tolerance);
}
