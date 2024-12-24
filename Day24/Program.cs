// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


Func<int, int, int> AND = (x, y) => x & y;
Func<int, int, int> OR = (x, y) => x | y;
Func<int, int, int> XOR = (x, y) => x ^ y;

var lines = File.ReadAllLines(args[0]);
Dictionary<string, int> reg = new();
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

Console.WriteLine("Let's go.");

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

Console.WriteLine("well, how was that?");
var allzs = reg.Keys.Where(x => x.StartsWith("z")).ToArray();
Array.Sort(allzs);

long result = 0;
for (int j = allzs.Length - 1; j >= 0; j--)
{
    result = result << 1; // shift one bit.
    var reggo = allzs[j];
    result += reg[reggo];
}
Console.WriteLine(result);

class Op
{
    public bool fired { get; set; } = false;
    public string leftoperand { get; set; }
    public string rightoperand { get; set; }
    public string outputreg { get; set; }
    public Func<int, int, int> op { get; set; }

}
