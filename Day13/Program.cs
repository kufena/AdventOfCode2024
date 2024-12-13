// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

static (int,int) SplitButtonLine(string aButtonLine)
{
    var splits = aButtonLine.Split(new char[] { ' ', ',', ':' }, StringSplitOptions.RemoveEmptyEntries);
    int xdiff = int.Parse(splits[2].Split("+")[1]);
    int ydiff = int.Parse(splits[3].Split("+")[1]);
    return (xdiff, ydiff);
}

var lines = File.ReadAllLines(args[0]);

int bCost = 1;
int aCost = 3;

for (int i = 0; i < lines.Length; i += 4)
{
    var aButtonLine = lines[i];
    var bButtonLines = lines[i + 1];
    var targetLine = lines[i + 2];

    (int xdiff_A, int ydiff_A) = SplitButtonLine(aButtonLine);
    (int xdiff_B, int ydiff_B) = SplitButtonLine(bButtonLines);

    var tsplits = targetLine.Split(new char[] { '=', ',' }, StringSplitOptions.RemoveEmptyEntries);
    var xTarget = int.Parse(tsplits[1]);
    var yTarget = int.Parse(tsplits[3]);

    Console.WriteLine($"First has targets {xTarget},{yTarget} A=({xdiff_A},{ydiff_A}) B=({xdiff_B},{ydiff_B})");

}
