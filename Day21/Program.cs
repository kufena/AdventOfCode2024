// See https://aka.ms/new-console-template for more information
using System.Diagnostics.CodeAnalysis;
using System.Net.NetworkInformation;
using System.Text;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

Dictionary<string, List<string>> memoNumericToKeypad = new();

Dictionary<char, (int, int)> keypadPositions =
    new();
keypadPositions.Add('7', (0, 0));
keypadPositions.Add('8', (0, 1));
keypadPositions.Add('9', (0, 2));
keypadPositions.Add('4', (1, 0));
keypadPositions.Add('5', (1, 1));
keypadPositions.Add('6', (1, 2));
keypadPositions.Add('1', (2, 0));
keypadPositions.Add('2', (2, 1));
keypadPositions.Add('3', (2, 2));
keypadPositions.Add('0', (3, 1));
keypadPositions.Add('A', (3, 2));

Dictionary<char, (int, int)> directionalPositions =
    new();
directionalPositions.Add('<', (1, 0));
directionalPositions.Add('v', (1, 1));
directionalPositions.Add('>', (1, 2));
directionalPositions.Add('^', (0, 1));
directionalPositions.Add('A', (0, 2));

Dictionary<string, List<string>> numkeypadMoves = new();

Dictionary<string, List<string>> numtokeyTranslations = new();
numtokeyTranslations.Add("AA", new List<string>() { "A" });
numtokeyTranslations.Add("A>", new List<string>() { "vA" });
numtokeyTranslations.Add("A<", new List<string>() { "<v<A", "v<<A"});
numtokeyTranslations.Add("A^", new List<string>() { "<A" });
numtokeyTranslations.Add("Av", new List<string>() { "v<A","<vA"});

numtokeyTranslations.Add(">A", new List<string>() { "^A" });
numtokeyTranslations.Add(">>", new List<string>() { "A" });
numtokeyTranslations.Add("><", new List<string>() { "<<A" });
numtokeyTranslations.Add(">^", new List<string>() { "<^A", "^<A"});
numtokeyTranslations.Add(">v", new List<string>() { "<A" });

numtokeyTranslations.Add("<A", new List<string>() { ">^>A", ">>^A" });
numtokeyTranslations.Add("<>", new List<string>() { ">>A" });
numtokeyTranslations.Add("<<", new List<string>() { "A" });
numtokeyTranslations.Add("<^", new List<string>() { ">^A" });
numtokeyTranslations.Add("<v", new List<string>() { ">A" });

numtokeyTranslations.Add("^A", new List<string>() { ">A" });
numtokeyTranslations.Add("^>", new List<string>() { "v>A", ">vA" });
numtokeyTranslations.Add("^<", new List<string>() { "v<A" });
numtokeyTranslations.Add("^^", new List<string>() { "A" });
numtokeyTranslations.Add("^v", new List<string>() { "vA" });

numtokeyTranslations.Add("vA", new List<string>() { ">^A", "^>A" });
numtokeyTranslations.Add("v>", new List<string>() { ">A" });
numtokeyTranslations.Add("v<", new List<string>() { "<A" });
numtokeyTranslations.Add("v^", new List<string>() { "^A" });
numtokeyTranslations.Add("vv", new List<string>() { "A" });

(int, int) five = (1, 1);

foreach (var keypos1 in keypadPositions)
{
    (char key1, (int pos1row, int pos1col)) = keypos1; 
    foreach (var keypos2 in keypadPositions)
    {
        (char key2, (int pos2row, int pos2col)) = keypos2;
        string newkey = $"{key1}{key2}";
        if (key1 == 'A' && key2 == '0')
            Console.WriteLine("");


        if (key1 == key2) // no path, just A
        {
            numkeypadMoves.Add(newkey, new List<string>() { "A" });
            continue;
        }

        if (pos1row == pos2row) // just cols change - one path.
        {
            StringBuilder sb = new();
            if (pos1col < pos2col)
            {
                for (int q = pos1col; q < pos2col; q++)
                {
                    sb.Append('>');
                }
            }
            else
            {
                for (int q = pos2col; q < pos1col; q++)
                {
                    sb.Append('<');
                }
            }
            sb.Append('A');
            numkeypadMoves.Add(newkey, new List<string>() { sb.ToString() });
            continue;
        }

        if (pos1col == pos2col) // just rows change - one path.
        {
            StringBuilder sb = new();
            if (pos1row < pos2row)
            {
                for (int q = pos1row; q < pos2row; q++)
                {
                    sb.Append('v');
                }
            }
            else
            {
                for (int q = pos2row; q < pos1row; q++)
                {
                    sb.Append('^');
                }
            }
            sb.Append('A');
            numkeypadMoves.Add(newkey, new List<string>() { sb.ToString() });
            continue;
        }

        int drow = 0;
        int dcol = 0;
        char rowchar = ' ';
        char colchar = ' ';
        if (pos2row < pos1row)
        {
            drow = -1;
            rowchar = '^';
        }
        else
        {
            drow = 1;
            rowchar = 'v';
        }
        if (pos2col < pos1col)
        {
            dcol = -1;
            colchar = '<';
        }
        else
        {
            dcol = 1;
            colchar = '>';
        }
        var res = EnumerateRoutes(pos1row, pos1col, pos2row, pos2col, drow, dcol, rowchar, colchar);
        numkeypadMoves.Add(newkey, res.ToList());
    }
}

HashSet<string> EnumerateRoutes(int row, int col, int destrow, int destcol, int drow, int dcol, char rowchar, char colchar)
{
    HashSet<string> routes = new HashSet<string>();
    if (row == 3 && col == 0) // it's the blank.
        return routes;

    if (row > 4 || row < 0 || col > 4 || col < 0)
        return routes;

    if (row == destrow && col == destcol) // we're there!
        routes.Add("A");
    else
    {
        var ans1 = EnumerateRoutes(row + drow, col, destrow, destcol, drow, dcol, rowchar, colchar);
        foreach (var l in ans1) routes.Add($"{rowchar}{l}");
        var ans2 = EnumerateRoutes(row, col + dcol, destrow, destcol, drow, dcol, rowchar, colchar);
        foreach (var l in ans2) routes.Add($"{colchar}{l}");
    }
    return routes;
}

//
//
//

Dictionary<(string, int), string> TwoByTwoMemo = new();
HashSet<int> depthsVisited = new();

// 22665
var twobtwo = TwoByTwo("A<", 3, true);
Console.WriteLine($"well? {twobtwo.Length}");
Console.WriteLine("How do you like them apples?");

//
//
//

long result = 0;
foreach (var line in lines)
{
    int startrow = 3;
    int startcol = 2;
    StringBuilder numericInstructions = new();
    StringBuilder resultbuilder = new();
    string withA = $"A{line}";
    Console.WriteLine(withA);
    memoNumericToKeypad = new();

    List<string> results = new() { "" };
    for (int k = 0; k < withA.Length - 1; k++)
    {
        string nummv = withA.Substring(k, 2);
        var numpaths = numkeypadMoves[nummv];
        List<string> newresults = new();
        foreach (var np in numpaths)
        {
            foreach (var rp in results)
            {
                newresults.Add($"{rp}{np}");
            }
        }
        results = newresults;
    }

    int minlen = int.MaxValue;
    string finalresult = "";
    foreach (var pref in results)
    {
        int pathlen = int.MaxValue;
        string chosen = "";
        Part2MalarkyRedone(directionalPositions, numtokeyTranslations, ref pathlen, ref chosen, $"A{pref}", 2);
        //Part2Malarky(directionalPositions, numtokeyTranslations, ref pathlen, ref chosen, pref);
        Console.WriteLine($"{pref} -> {chosen} {chosen.Length}");
        if (chosen.Length < minlen)
        {
            minlen = chosen.Length;
            finalresult = chosen.Substring(1); // drop the starting A.
        }
    }

    //for (int k = 0; k < withA.Length-1; k++)
    //{
    //    // Do each digit at a time. They all end on A, so
    //    // the keypad steps will all end in the same place.
    //    string nummv = withA.Substring(k, 2);
    //    var numpaths = numkeypadMoves[nummv];
    //    Console.WriteLine($"{nummv} gives {numpaths.Count} paths");
    //    foreach (var nmp in numpaths) Console.Write($"{nmp} ");
    //    Console.WriteLine("");
    //    int pathlen = int.MaxValue;
    //    string chosen = "";
    //    foreach (var prefix in numpaths)
    //    {
    //        Part1Malarky(directionalPositions, numtokeyTranslations, ref pathlen, ref chosen, prefix);
    //        //Part2Malarky(directionalPositions, numtokeyTranslations, ref pathlen, ref chosen, prefix);
    //    }
    //    resultbuilder.Append(chosen);
    //}
//    string finalresult = resultbuilder.ToString();
    Console.WriteLine($"{finalresult}  {finalresult.Length}");
    int q = int.Parse(line.Substring(0,line.Length - 1));
    Console.WriteLine($"{line.Substring(0, line.Length - 1)} {q * finalresult.Length}");
    result += (long)(q * finalresult.Length);
    Console.WriteLine("");
}

Console.WriteLine(result);

(int newstartrow, int newstartcol, string prefix) NumericMove(int startrow, int startcol, char x)
{
    StringBuilder sb = new();
    (int xrow, int xcol) = keypadPositions[x];

    // This palaver is all to avoid the blank square at the bottom left of
    // the numeric keypad. It's a simple heuristic to avoid it.
    if (xcol < startcol) // we're going left so go up or down first.
    {
        int numupdown = Math.Abs(xrow - startrow);
        if (xrow < startrow)
        {
            for (int i = 0; i < numupdown; i++) sb.Append('^');
        }
        else
        {
            for (int i = 0; i < numupdown; i++) sb.Append('v');         
        }
        for (int i = 0; i < Math.Abs(xcol - startcol); i++)
            sb.Append('<');
    }
    else
    {
        for (int i = 0; i < Math.Abs(xcol - startcol); i++)
            sb.Append('>');
        for (int i = 0; i < Math.Abs(xrow - startrow); i++)
        {
            if (xrow < startrow) sb.Append("^");
            else sb.Append("v");
        }
    }
    sb.Append("A");
    return (xrow, xcol, sb.ToString());
}

// This middle step needs to enumerate all the paths between a number
// of instructions, so that we can choose the one that encodes to the
// smallest set of instructions in the next step.
//
// I think, though, that as they all end in A, each numeric is on its
// own, so they don't interact.
List<string> NumericToKeypad(string instructions)
{
    if (memoNumericToKeypad.ContainsKey(instructions))
        return memoNumericToKeypad[instructions];

    // numtokeyTranslations does all the work here.
    instructions = $"A{instructions}";
    List<string> sofar = new List<string>() { "" };
    for (int i = 1; i < instructions.Length; i++)
    {
        string sb = instructions.Substring(i - 1, 2);
        List<string> trans = numtokeyTranslations[sb];
        List<string> sofarsofar = new List<string>();
        foreach (var p in trans)
        {
            foreach (var t in sofar)
            {
                sofarsofar.Add($"{t}{p}");
            }
        }
        sofar = sofarsofar;
    }
    memoNumericToKeypad.Add(instructions, sofar);
    return sofar;
}

List<string> NumericToKeypad2(string instructions)
{
    if (memoNumericToKeypad.ContainsKey(instructions))
        return memoNumericToKeypad[instructions];

    int Acount = 0;
    for (int q = 0; q < instructions.Length; q++)
    {
        if (instructions[q] == 'A')
        {
            Acount++;
            //if (Acount > 1) // ie 2 or more.
            //    break;
        }
    }

    if (Acount <= 1)
    {

        // numtokeyTranslations does all the work here.
//        instructions = instructions.StartsWith("A") ? instructions : $"A{instructions}";
        instructions = $"A{instructions}";

        if (memoNumericToKeypad.ContainsKey(instructions))
            return memoNumericToKeypad[instructions];

        List<string> sofar = new List<string>() { "" };
        for (int i = 1; i < instructions.Length; i++)
        {
            string sb = instructions.Substring(i - 1, 2);
            List<string> trans = numtokeyTranslations[sb];
            List<string> sofarsofar = new List<string>();
            foreach (var p in trans)
            {
                foreach (var t in sofar)
                {
                    sofarsofar.Add($"{t}{p}");
                }
            }
            sofar = sofarsofar;
        }
        if (sofar.Count > 1)
        {
            int largest = int.MaxValue;
            foreach (var sf in sofar)
            {
                if (sf.Length < largest) largest = sf.Length;
            }
            sofar = sofar.Where(x => x.Length == largest).ToList();
            //sofar = new List<string>() { sofar.First() };
        }

        memoNumericToKeypad.Add(instructions, sofar);
        return sofar;
    }
    else
    {
        // we assume an A at the end.
        if (!instructions.EndsWith("A")) throw new Exception($"No A at the end of instruction block {instructions}");

        string[] splits = new string[Acount];
        int count = 0;
        int pos = 0;
        int start = 0;
        while (pos < instructions.Length)
        {
            if (instructions[pos] == 'A')
            {
                splits[count] = instructions.Substring(start, (pos - start) + 1);
                count++;
                start = pos + 1;
            }
            pos++;
        }

        HashSet<string> sofar = new HashSet<string>() { "" };
        for (int i = 0; i < splits.Length; i++)
        {
            //splits[i] = $"{splits[i]}A";
            var t = NumericToKeypad2(splits[i]);
            HashSet<string> newsofar = new();
            if (sofar.Count == 0)
                return sofar.ToList();
            int len = int.MaxValue;
            foreach (var l in sofar)
                foreach (var b in t)
                {
                    string s = $"{l}{b}";
                    if (s.Length < len) len = s.Length;
                    newsofar.Add(s);
                }
            sofar = newsofar; // new HashSet<string>() { newsofar.Where(x => x.Length == len).First() };
        }
        //memoNumericToKeypad.Add(instructions, sofar.ToList());
        return sofar.ToList();
    }
}


string KeypadToKeypad(string instructions)
{
    StringBuilder sb = new();
    (int startrow, int startcol) = directionalPositions['A'];
    for (int i = 0; i < instructions.Length; i++)
    {
        (int irow, int icol) = directionalPositions[instructions[i]];
        if (irow < startrow) // we're going up, so go left or right first.
        {
            for (int ji = 0; ji < Math.Abs(startcol - icol); ji++)
            {
                if (icol < startcol) sb.Append('<');
                else sb.Append('>');
            }
            for (int ji = 0; ji < Math.Abs(startrow - irow); ji++)
            {
                sb.Append('^');
            }
        }
        else // go up/down first.
        {
            for (int ji = 0; ji < Math.Abs(startrow - irow); ji++)
            {
                sb.Append('v');
            }
            for (int ji = 0; ji < Math.Abs(startcol - icol); ji++)
            {
                if (icol < startcol) sb.Append('<');
                else sb.Append('>');
            }
        }
        sb.Append('A');
        startrow = irow;
        startcol = icol;
    }
    return sb.ToString();
}

// Part2 Malarky is 25 times Part1 Malarky really.
void Part2MalarkyRedone(Dictionary<char, (int, int)> directionalPositions, Dictionary<string, List<string>> numtokeyTranslations, ref int pathlen, ref string chosen, string prefix, int iters)
{
    string s = TwoByTwo(prefix, iters, true );

    chosen = s;
    pathlen = s.Length;
}

void Part2Malarky(Dictionary<char, (int, int)> directionalPositions, Dictionary<string, List<string>> numtokeyTranslations, ref int pathlen, ref string chosen, string prefix)
{
    int iters = 25; // two should give part1 answer, we need 25 iterations though.
    HashSet<string> subMalarky = new HashSet<string>() { prefix };
    for (int i = 0; i < iters; i++)
    {
        HashSet<string> newbunch = new();
        foreach (var pre in subMalarky)
        {
            List<string> thislot = NumericToKeypad2($"A{pre}");
            foreach (var t in thislot) newbunch.Add(t);
        }

        // prune for shortest?
        //int sht = int.MaxValue;
        //foreach (var t in newbunch)
        //{
        //    if (t.Length < sht) sht = t.Length;
        //}
        //subMalarky = new HashSet<string>() { newbunch.Where(x => x.Length == sht).First() };
        subMalarky = newbunch;
    }

    var possiblePaths = subMalarky;

    foreach (string path in possiblePaths)
    {
        string finalencoding = KeypadToKeypad(path);
        if (finalencoding.Length < pathlen)
        {
            chosen = finalencoding;
            pathlen = finalencoding.Length;
        }
    }
}

void Part1Malarky(Dictionary<char, (int, int)> directionalPositions, Dictionary<string, List<string>> numtokeyTranslations, ref int pathlen, ref string chosen, string prefix)
{
    List<string> possiblePaths = NumericToKeypad(prefix);
    foreach (string path in possiblePaths)
    {
        string finalencoding = KeypadToKeypad(path);
        if (finalencoding.Length < pathlen)
        {
            chosen = finalencoding;
            pathlen = finalencoding.Length;
        }
    }
}

string TwoByTwo(string s, int depth, bool top)
{
    if (depth == 0)
    {
        return s;
    }

    if (TwoByTwoMemo.ContainsKey((s,depth)))
        return TwoByTwoMemo[(s,depth)];

    bool addA = !depthsVisited.Contains(depth);
    if (addA) depthsVisited.Add(depth);

    //if (!depthsVisited.Contains(depth))
    //{
    //    s = $"A{s}";
    //    depthsVisited.Add(depth);
    //}

    List<string> results = new List<string>() { "" };
    for (int i = 1; i < s.Length; i++)
    {
        string substring = s.Substring(i - 1, 2);
        var todo = numtokeyTranslations[substring];
        List<string> newresults = new();
        foreach (var doer in todo)
        {
            string cand = doer;
            if (substring.StartsWith("A") && !doer.StartsWith("A")) cand = $"A{doer}";
            var subs = TwoByTwo(cand, depth - 1, true);
            newresults.Add(subs);
        }

        List<string> finals = new();
        foreach (var nrs in newresults)
        {
            foreach (var srn in results)
            {
                finals.Add($"{srn}{nrs}");
            }
        }
        results = finals;
    }

    var val = results.Order().First();
    TwoByTwoMemo.Add((s, depth), val);
    return val;
    //return results;
}
