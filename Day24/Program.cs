// See https://aka.ms/new-console-template for more information
using System.ComponentModel.Design;
using System.Reflection;

Console.WriteLine("Hello, World!");


Func<int, int, int> AND = (x, y) => x & y;
Func<int, int, int> OR = (x, y) => x | y;
Func<int, int, int> XOR = (x, y) => x ^ y;

var lines = File.ReadAllLines(args[0]);
Dictionary<string, int> reg = new();
Dictionary<string, int> regCopy = new();
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

foreach (var p in reg) regCopy.Add(p.Key, p.Value);

Console.WriteLine($"Let's go. {allops.Count} ops.");
var emptyExchanges = new Dictionary<string, string>();

//emptyExchanges.Add("z05", "z00");
//emptyExchanges.Add("z00", "z05");
//emptyExchanges.Add("z02", "z01");
//emptyExchanges.Add("z01", "z02");

RunOne(reg, allops, emptyExchanges);

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
//Console.WriteLine($"{xval} + {yval} = {xval & yval} diffs {(xval & yval) ^ result}");
Console.WriteLine($"{xval} + {yval} = {xval + yval} diffs {(xval + yval) ^ result}");
Console.ReadKey();

List<string>[] start = new List<string>[allzs.Length];
for (int ii = 0; ii < allzs.Length; ii++)
{
    start[ii] = findStarts(depends, allzs[ii]);
    //Console.WriteLine($"reg {allzs[ii]} depends on");
    //foreach (var s in start[ii]) Console.WriteLine($"    {s}");
    //Console.WriteLine();
}

HashSet<string> potentials = new();
HashSet<string> potentials2 = new();
bool first = true;
bool first2 = true;
for (int nt = 0; nt < allxs.Length; nt++)
{
    string xtest = $"x{nt.ToString("D2")}";
    string ytest = $"y{nt.ToString("D2")}";

    var newreg = new Dictionary<string, int>();
    foreach (var k in regCopy.Keys)
    {
        if (k.StartsWith("x") || k.StartsWith("y"))
            newreg.Add(k, 0);
        else
            newreg.Add(k, -1);
    }
    newreg[xtest] = 0;
    newreg[ytest] = 1;

    HashSet<string> mypots = new();

    RunOne(newreg, allops, emptyExchanges);
    var newresult = BitsToLong(newreg, allzs);
    var newx = BitsToLong(newreg, allxs);
    var newy = BitsToLong(newreg, allys);
    ResetOps(allops);

    if (newresult != (newx + newy))
    {
        Console.WriteLine($"{nt} seems to cause issues. {newx} {newy} {newresult}");
        var xdiffers = DisplayBitDifferences(newx, newy, newresult, allzs.Length);
        foreach (var p in xdiffers)
        {
            foreach (var g in depends[p])
                mypots.Add(g);
            mypots.Add(p);
        }

        foreach (var p in mypots) potentials.Add(p);
        //if (first)
        //{
        //    potentials = mypots;
        //    first = false;
        //}
        //else
        //{
        //    potentials = potentials.Intersect(mypots).ToHashSet();
        //}
    }

    foreach (var k in newreg.Keys)
    {
        if (k.StartsWith("x") || k.StartsWith("y"))
            newreg[k] = 0;
        else
            newreg[k] = -1;
    }
    newreg[xtest] = 1;
    newreg[ytest] = 1;

    RunOne(newreg, allops, emptyExchanges);
    newresult = BitsToLong(newreg, allzs);
    newx = BitsToLong(newreg, allxs);
    newy = BitsToLong(newreg, allys);
    ResetOps(allops);

    if (newresult != (newx + newy))
    {
        Console.WriteLine($"{nt} seems to cause issues. {newx} {newy} {newresult}");
        var ydiffers = DisplayBitDifferences(newx, newy, newresult, allzs.Length);
        mypots = new();
        foreach (var p in ydiffers)
        {
            foreach (var g in depends[p])
                mypots.Add(g);
            mypots.Add(p);
        }
        foreach (var p in mypots) potentials.Add(p);

        //if (first2)
        //{
        //    potentials2 = mypots;
        //    first2 = false;
        //}
        //else
        //{
        //    potentials2 = potentials2.Union(mypots).ToHashSet();
        //}
    }
}

foreach (var t in potentials2) potentials.Add(t);
Console.WriteLine($"{potentials.Count} possible exchanges.");
foreach (var t in potentials) Console.Write($"{t} ");
Console.ReadKey();

HashSet<(string, string)> allpairs = new();
//foreach (var pot1 in potentials)
//{
//    foreach (var pot2 in potentials)
//    {
//        if (pot1 != pot2)
//            allpairs.Add((pot1, pot2));
//    }
//}
var potentialsArr = potentials.ToArray();
for (int iii = 0; iii < potentials.Count; iii++)
{
    for (int iij = iii+1; iij < potentials.Count; iij++)
    {
        allpairs.Add((potentialsArr[iii], potentialsArr[iij]));
    }
}

int c = 0;
var allpairsArr = allpairs.ToArray();
//foreach (var p1 in allpairs)
for(int apai = 0; apai < allpairsArr.Length; apai++)
{
    var p1 = allpairsArr[apai];
//    foreach (var p2 in allpairs)
    for(int apaj = apai+1; apaj < allpairsArr.Length; apaj++)
    {
        var p2 = allpairsArr[apaj];
//        foreach (var p3 in allpairs)
        for(int apak = apaj+1; apak < allpairsArr.Length; apak++)
        {
            var p3 = allpairsArr[apak];
//            foreach (var p4 in allpairs)
            for(int apan = apak+1; apan < allpairsArr.Length; apan++)
            {
                var p4 = allpairsArr[apan];
                c++;
                HashSet<string> bodies = new();
                bodies.Add(p1.Item1); bodies.Add(p1.Item2);
                bodies.Add(p2.Item1); bodies.Add(p2.Item2);
                bodies.Add(p3.Item1); bodies.Add(p3.Item2);
                bodies.Add(p4.Item1); bodies.Add(p4.Item2);
                if (bodies.Count == 8)
                {
                    //Console.WriteLine($"Trying one at {c}");
                    //allfours.Add((p1, p2, p3, p4));
                    Dictionary<string, string> exchanges = new Dictionary<string, string>();
                    exchanges.Add(p1.Item1, p1.Item2);
                    exchanges.Add(p2.Item1, p2.Item2);
                    exchanges.Add(p3.Item1, p3.Item2);
                    exchanges.Add(p4.Item1, p4.Item2);
                    exchanges.Add(p1.Item2, p1.Item1);
                    exchanges.Add(p2.Item2, p2.Item1);
                    exchanges.Add(p3.Item2, p3.Item1);
                    exchanges.Add(p4.Item2, p4.Item1);
                    Dictionary<string, int> myreg = new();
                    foreach (var r in regCopy) myreg.Add(r.Key, r.Value);
                    RunOne(myreg, allops, exchanges);
                    ulong myres = BitsToLong(myreg, allzs);
                    ulong myx = BitsToLong(myreg, allxs);
                    ulong myy = BitsToLong(myreg, allys);

                    if (myres == (myx + myy))
                    {
                        Console.WriteLine($"{p1.Item1} <-> {p1.Item2} {p2.Item1} <-> {p2.Item2} {p3.Item1} <-> {p3.Item2} {p4.Item1} <-> {p4.Item2}");
                        Console.ReadKey();
                    }
                }
                if (c % 10000 == 0) Console.WriteLine(c);
            }
        }
    }
}

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

static void RunOne(Dictionary<string, int> reg, List<Op> allops, Dictionary<string,string> exchanges)
{
    bool changed = true;
    while (changed)
    {
        changed = false;
        foreach (var o in allops)
        {
            if (o.fired)
                continue;

            string leftop = o.leftoperand;
            string rightop = o.rightoperand;

            string outreg = o.outputreg;
            if (exchanges.ContainsKey(outreg))
                outreg = exchanges[outreg];

            if (reg[leftop] != -1 && reg[rightop] != -1)
            {
                reg[outreg] = o.op(reg[leftop], reg[rightop]);
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

static HashSet<string> DisplayBitDifferences(ulong xval, ulong yval, ulong result, int zcount)
{
    ulong mask = 1;
    for (int i = 0; i < zcount; i++) mask = mask << 1;

    ulong expect = xval + yval;
    int count = zcount;
    HashSet<string> diffbits = new();
    while (mask > 0)
    {
        if ((expect & mask) != (result & mask))
        {
            string zer = $"z{count.ToString("D2")}";
            Console.WriteLine($"    z{count.ToString("D2")}");
            diffbits.Add(zer);
        }
        count--;
        mask = mask >> 1;
    }
    return diffbits;
}

class Op
{
    public bool fired { get; set; } = false;
    public string leftoperand { get; set; }
    public string rightoperand { get; set; }
    public string outputreg { get; set; }
    public Func<int, int, int> op { get; set; }

}
