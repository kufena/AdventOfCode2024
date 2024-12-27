// See https://aka.ms/new-console-template for more information
using System.ComponentModel.Design;
using System.Reflection;

Console.WriteLine("Hello, World!");


Func<int, int, int> AND = (x, y) => x & y;
Func<int, int, int> OR = (x, y) => x | y;
Func<int, int, int> XOR = (x, y) => x ^ y;

var lines = File.ReadAllLines(args[0]);
Dictionary<string, int> reg = new();
Dictionary<string, List<string>> depends = new();
List<Op> allops = new();

int i = 0;
int blank = 0;

while (lines[i].Trim() != "")
{
    var stilps = lines[i].Split(':', StringSplitOptions.RemoveEmptyEntries);
    reg.Add(stilps[0], int.Parse(stilps[1]));
    i++;
}
blank = i;
i++; // skip the blank line.

while (i < lines.Length)
{
    var splits = lines[i].Split(' ', StringSplitOptions.RemoveEmptyEntries);
    i++;
    Op bauble;
    depends.Add(splits[4], new List<string>() { splits[0], splits[2] });
    switch (splits[1])
    {
        case "AND":
            bauble = new Op()
            {
                leftoperand = splits[0],
                rightoperand = splits[2],
                outputreg = splits[4],
                op = AND
            };
            break;
        case "OR":
            bauble = new Op()
            {
                leftoperand = splits[0],
                rightoperand = splits[2],
                outputreg = splits[4],
                op = OR
            };
            break;
        case "XOR":
            bauble = new Op()
            {
                leftoperand = splits[0],
                rightoperand = splits[2],
                outputreg = splits[4],
                op = XOR
            };
            break;
        default:
            throw new Exception($"don't know {splits[1]}");
    }
    allops.Add(bauble);
    if (!reg.ContainsKey(splits[0])) reg.Add(splits[0], -1);
    if (!reg.ContainsKey(splits[2])) reg.Add(splits[2], -1);
    if (!reg.ContainsKey(splits[4])) reg.Add(splits[4], -1);
}

Console.WriteLine($"Let's go. {allops.Count} ops.");

RunOne(reg, allops);

ResetOps(allops);

Console.WriteLine("well, how was that?");
var allxs = reg.Keys.Where(x => x.StartsWith("x")).ToArray();
Array.Sort(allxs);
var allys = reg.Keys.Where(x => x.StartsWith("y")).ToArray();
Array.Sort(allys);
var allzs = reg.Keys.Where(x => x.StartsWith("z")).ToArray();
Array.Sort(allzs);

ulong xval = BitsToLong(reg, allxs);
ulong yval = BitsToLong(reg, allys);
ulong result = BitsToLong(reg, allzs);
Console.WriteLine(result);
Console.WriteLine($"{xval} + {yval} = {xval + yval} diffs {(xval + yval) ^ result}");

List<string>[] start = new List<string>[allzs.Length];
for (int ii = 0; ii < allzs.Length; ii++)
{
    start[ii] = findStarts(depends, allzs[ii]);
    //Console.WriteLine($"reg {allzs[ii]} depends on");
    //foreach (var s in start[ii]) Console.WriteLine($"    {s}");
    //Console.WriteLine();
}

//ulong duffbits = (xval + yval) ^ result;
//int count = 0;
//List<string> diffbits = new();
//while (duffbits > 0)
//{
//    if (duffbits % 2 == 1)
//    {
//        string zer = $"z{count.ToString("D2")}";
//        diffbits.Add(zer);
//        count++;
//        duffbits -= 1;
//    }
//    duffbits = duffbits >> 1;
//}

for (int nt = 0; nt < allxs.Length; nt++)
{
    string xtest = $"x{nt.ToString("D2")}";
    string ytest = $"y{nt.ToString("D2")}";

    var newreg = new Dictionary<string, int>();
    foreach (var k in reg.Keys)
    {
        if (k.StartsWith("x") || k.StartsWith("y"))
            newreg.Add(k, 0);
        else
            newreg.Add(k, -1);
    }
    newreg[xtest] = 1;
    newreg[ytest] = 1;

    RunOne(newreg, allops);
    var newresult = BitsToLong(newreg, allzs);
    var newx = BitsToLong(newreg, allxs);
    var newy = BitsToLong(newreg, allys);
    ResetOps(allops);


    if (newresult != (newx + newy))
        Console.WriteLine($"{nt} seems to cause issues. {newx} {newy} {newresult}");
}

HashSet<string> totest = new();


Console.WriteLine("");

static List<string> findStarts(Dictionary<string, List<string>> depends, string outp)
{
    List<string> res = new();
    var l = depends[outp];
    foreach (var ll in l)
    {
        res.Add(ll);
        if (!(ll.StartsWith("x") || ll.StartsWith("y")))
        {
            var subres = findStarts(depends, ll);
            foreach (var sr in subres) res.Add(sr);
        }
    }
    return res;
}

static ulong BitsToLong(Dictionary<string, int> reg, string[] allzs)
{
    ulong result = 0;
    for (int j = allzs.Length - 1; j >= 0; j--)
    {
        result = result << 1; // shift one bit.
        var reggo = allzs[j];
        result += (ulong)reg[reggo];
    }

    return result;
}

static void RunOne(Dictionary<string, int> reg, List<Op> allops)
{
    bool changed = true;
    while (changed)
    {
        changed = false;
        foreach (var o in allops)
        {
            if (o.fired)
                continue;
            if (reg[o.leftoperand] != -1 && reg[o.rightoperand] != -1)
            {
                reg[o.outputreg] = o.op(reg[o.leftoperand], reg[o.rightoperand]);
                o.fired = true;
                changed = true;
            }
        }
    }
}

static void ResetOps(List<Op> allops)
{
    foreach (var op in allops)
    {
        op.fired = false;
    }
}

class Op
{
    public bool fired { get; set; } = false;
    public string leftoperand { get; set; }
    public string rightoperand { get; set; }
    public string outputreg { get; set; }
    public Func<int, int, int> op { get; set; }

}
