// See https://aka.ms/new-console-template for more information
using System.Net.NetworkInformation;
using System.Text;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

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

(int, int) five = (1, 1);

KeypadToKeypad("<A>A<AAv<AA>>^AvAA^Av<AAA>^A");

long result = 0;
foreach (var line in lines)
{
    Console.WriteLine(line);
    int startrow = 3;
    int startcol = 2;
    StringBuilder numericInstructions = new();
    for (int k = 0; k < line.Length; k++)
    {
        (int newstartrow, int newstartcol, string prefix) = NumericMove(startrow, startcol, line[k]);
        numericInstructions.Append(prefix);
        startrow = newstartrow;
        startcol = newstartcol;
    }
    Console.WriteLine(numericInstructions.ToString());
    string keyresult = KeypadToKeypad(numericInstructions.ToString());
    Console.WriteLine($"{keyresult.Length} |{keyresult}|");
    keyresult = KeypadToKeypad(keyresult);
    Console.WriteLine($"{keyresult.Length} |{keyresult}|");
    int q = int.Parse(line.Substring(0,line.Length - 1));
    Console.WriteLine($"{line.Substring(0, line.Length - 1)} {q * keyresult.Length}");
    result += (long)(q * keyresult.Length);
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