// See https://aka.ms/new-console-template for more information

using System.Runtime;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);
int rows = lines.Length;
int cols = lines[0].Length;

Dictionary<int, List<(int, int)>> plotsToPoints = new();
int[][] plots = new int[rows][];
int plotIds = 0;

for (int i = 0; i < rows; i++)
{
    plots[i] = new int[cols];
    for (int j = 0; j < cols; j++)
    {
        //if (i == 3 && j == 9)
        //{
        //    Console.WriteLine("woops");
        //}

        // check behind and above.
        char behind = getBehind(i, j, lines);
        char above = getAbove(i, j, lines);

        char hereAndNow = lines[i][j];
        if (hereAndNow == behind && hereAndNow == above)
        {
            // they're the same plot, possibly need to merge plots.
            if (plots[i - 1][j] == plots[i][j - 1]) // they're the same group!
            {
                plots[i][j] = plots[i - 1][j];
                plotsToPoints[plots[i - 1][j]].Add((i, j));
            }
            else // merge!
            {
                int plotBehind = plots[i - 1][j];
                int plotAbove = plots[i][j - 1];
                foreach ((int y, int x) in plotsToPoints[plotBehind])
                {
                    plots[y][x] = plotAbove;
                    plotsToPoints[plotAbove].Add((y, x));
                }
                plotsToPoints.Remove(plotBehind);
                plots[i][j] = plotAbove;
                plotsToPoints[plotAbove].Add((i, j));
            }
        }
        else if (hereAndNow == behind)
        {
            plots[i][j] = plots[i][j-1];
            plotsToPoints[plots[i][j]].Add((i, j));
        }
        else if (hereAndNow == above)
        {
            plots[i][j] = plots[i-1][j];
            plotsToPoints[plots[i][j]].Add((i, j));
        }
        else
        {
            int myPlot = plotIds;
            plotIds++;
            plotsToPoints.Add(myPlot, new List<(int, int)>() { (i, j) });
            plots[i][j] = myPlot;
        }

        if (!plotsToPoints.ContainsKey(plots[i][j]))
        {
            Console.WriteLine("duffer");
        }
    }
}

Console.WriteLine($"{plotsToPoints.Keys.Count} plots");

long price = 0;
long discountPrice = 0;
foreach (var k in plotsToPoints.Keys)
{
    (int y, int x) = plotsToPoints[k].First();
    char c = lines[y][x];
    var points = plotsToPoints[k];
    int area = points.Count;
    int perim = perimeter(points, lines, rows, cols);
    int sides = countSides(points, lines, rows, cols);
    Console.WriteLine($"Plot {k} ({c}) has area {area} and perimeter {perim} and {sides} sides");
    price += (long)(area * perim);
    discountPrice += (long)(area * sides);
}

Console.WriteLine($"Price is {price} but Discount Price is {discountPrice}");

int perimeter(List<(int,int)> points, string[] lines, int rows, int cols)
{

    int totalPerim = 0;
    foreach ((int y, int x) in points)
    {
        if (y - 1 < 0) totalPerim++;
        else totalPerim += (lines[y - 1][x] == lines[y][x]) ? 0 : 1;
        if (y + 1 >= rows) totalPerim++;
        else totalPerim += (lines[y + 1][x] == lines[y][x]) ? 0 : 1;
        if (x - 1 < 0) totalPerim++;
        else totalPerim += (lines[y][x - 1] == lines[y][x]) ? 0 : 1;
        if (x + 1 >= cols) totalPerim++;
        else totalPerim += (lines[y][x + 1] == lines[y][x]) ? 0 : 1;
    }
    return totalPerim;
}

//
// We should count corners!
//
int countSides(List<(int,int)> points, string[] lines, int rows, int cols)
{
    int corners = 0;

    if (points.Count == 1) return 4;

    foreach ((int y, int x) in points)
    {
        char pc = lines[y][x];
        char behind = getBehind(y, x, lines);
        char infront = getInfront(y, x, lines);
        char above = getAbove(y, x, lines);
        char below = getBelow(y, x, lines);
        char upperleft = upperLeft(y, x, lines);
        char upperright = upperRight(y, x, lines);
        char lowerleft = lowerLeft(y, x, lines);
        char lowerright = lowerRight(y, x, lines);


        //if (y == 0 && x == 0) corners++; // top left
        //if (y == 0 && x + 1 == cols) corners++; // top right
        //if (y + 1 == rows && x == 0) corners++; // bottom left
        //if (y + 1 == rows && x + 1 == cols) corners++; // bottom right

        if (above == pc && infront == pc && upperright != pc) corners++;
        if (above == pc && behind == pc && upperleft != pc) corners++;
        if (below == pc && infront == pc && lowerright != pc) corners++;
        if (below == pc && behind == pc && lowerleft != pc) corners++;

        if (above != pc && behind != pc) corners++;
        if (above != pc && infront != pc) corners++;
        if (below != pc && behind != pc) corners++;
        if (below != pc && infront != pc) corners++;


    }

    return corners;
}

//
// Utilities that effectively make the grid have an extra layer on top/bottom
// and to the sides. I should probably have done corners here too.

char getAbove(int i, int j, string[] lines)
{
    if (i - 1 < 0) return '!';
    return lines[i - 1][j];
}

char getBehind(int i, int j, string[] lines)
{
    if (j - 1 < 0) return ')';
    return lines[i][j - 1];
}

char getBelow(int i, int j, string[] lines)
{
    if (i + 1 >= lines.Length) return '£';
    return lines[i + 1][j];
}

char getInfront(int i, int j, string[] lines)
{
    if (j + 1 >= lines[i].Length) return '(';
    return lines[i][j + 1];
}

char lowerLeft(int i, int j, string[] lines)
{
    if (j-1 < 0 || i+1 >= lines.Length) return '*';
    return lines[i + 1][j - 1];
}

char lowerRight(int i, int j, string[] lines)
{
    if (j + 1 >= lines[i].Length || i + 1 >= lines.Length) return '?';
    return lines[i + 1][j + 1];
}

char upperLeft(int i, int j, string[] lines)
{
    if (i - 1 < 0 || j - 1 < 0) return '|';
    return lines[i - 1][j - 1];
}

char upperRight(int i, int j, string[] lines)
{
    if (i - 1 < 0 || j + 1 >= lines[i].Length) return '>';
    return lines[i - 1][j + 1];
}


