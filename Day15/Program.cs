// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
var lines = File.ReadAllLines(args[0]);
int k = 0;

List<string> gridLines = new();
while (lines[k] != "")
{
    gridLines.Add(lines[k]);
    k++;
}
k++;
List<string> directions = new();
while (k < lines.Length)
{
    directions.Add(lines[k]);
    k++;
}

int rows = gridLines.Count;
int cols = gridLines.First().Length;

Console.WriteLine($"{rows} rows by {cols} columns.");

int robotx = 0;
int roboty = 0;
char[][] grid = new char[rows][];
for (int i = 0; i < rows; i++)
{
    var line = lines[i];
    grid[i] = new char[cols];
    for (int j = 0; j < cols; j++)
    {
        if (line[j] == '@') // it's the robot.
        {
            robotx = j;
            roboty = i;
            grid[j][i] = '.';
        }
        else
        {
            grid[i][j] = line[j];
        }
    }
}

displayGrid(grid, rows, cols, robotx, roboty);

foreach (var dirLine in directions) 
{
    for (int q = 0; q < dirLine.Length; q++)
    {
        (int dx, int dy) = dirToChange(dirLine[q]);
        if (doPush(dx, dy, robotx, roboty, grid))
        {
            robotx += dx;
            roboty += dy;
            Console.WriteLine($"Direction {dirLine[q]} yields a move.");
            displayGrid(grid, rows, cols, robotx, roboty);
        }
        else
        {
            Console.WriteLine($"Direction {dirLine[q]} no movement.");
            displayGrid(grid, rows, cols, robotx, roboty);
        }
        //Console.ReadKey();
        //Console.Clear();
    }
}

var total = lanternFishCalculation(grid, rows, cols);
Console.WriteLine($"Total is {total}");

void displayGrid(char[][] grid, int rows, int cols, int robotx, int roboty)
{
    //for (int i = 0; i < rows; i++)
    //{
    //    for (int j = 0; j < cols; j++)
    //    {
    //        if (i == roboty && j == robotx)
    //            Console.Write('@');
    //        else
    //            Console.Write(grid[i][j]);
    //    }
    //    Console.WriteLine("");
    //}
}

bool doPush(int dx, int dy, int x, int y, char[][] grid)
{
    int nextx = x + dx;
    int nexty = y + dy;
    if (grid[nexty][nextx] == '.')
    {
        grid[nexty][nextx] = grid[y][x];
        grid[y][x] = '.';
        return true;
    }
    if (grid[nexty][nextx] == '#')
    {
        return false;
    }
    // in other words, if it's an 'O' can we push that?
    if (doPush(dx, dy, nextx, nexty, grid))
    {
        grid[nexty][nextx] = grid[y][x];
        grid[y][x] = '.';
        return true;
    }

    return false;
}

// this return (dx,dy)! Note.
(int,int) dirToChange(char c)
{
    switch (c)
    {
        case '^':
            return (0, -1);
        case '>':
            return (1, 0);
        case '<':
            return (-1, 0);
        case 'v':
            return (0, 1);
        default:
            throw new ArgumentException($"{c} not a valid direction.");
    }
}

long lanternFishCalculation(char[][] grid, int rows, int cols)
{
    long total = 0;
    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            if (grid[i][j] == 'O')
            {
                total += (100 * i) + j;
            }
        }
    }
    return total;
}
