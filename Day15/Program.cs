// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

static void ExpandGrid(string[] lines, int rows, int cols, ref int robotx, ref int roboty, HashSet<(int, int)> wallBlocks, HashSet<(int, int)> boxBlocks)
{
    for (int i = 0; i < rows; i++)
    {
        var line = lines[i];
        //        grid[i] = new char[cols*2];
        for (int j = 0; j < cols; j++)
        {
            if (line[j] == '@') // it's the robot.
            {
                robotx = j * 2;
                roboty = i;
                //grid[i][j*2] = '@';
                //grid[i][(j*2) + 1] = '.';
            }
            else if (line[j] == 'O')
            {
                //grid[i][j*2] = '[';
                //grid[i][(j*2) + 1] = ']';
                boxBlocks.Add((i, j * 2)); // just record the [ bit.
            }
            else if (line[j] == '#')
            {
                wallBlocks.Add((i, j * 2));
                wallBlocks.Add((i, (j * 2) + 1));
                //grid[i][j * 2] = line[j];
                //grid[i][(j * 2) + 1] = line[j];
            }
            else
            {
                //grid[i][j * 2] = line[j];
                //grid[i][(j * 2) + 1] = line[j];
            }
        }
    }
}

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

//Part1(lines, directions, rows, cols);
Part2(lines, directions, rows, cols);

void displayPosiGrid(HashSet<(int,int)> wallBlocks, HashSet<(int,int)> boxBlocks, int robx, int roby, int cols, int rows)
{
    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            if (i == roby && j == robx)
                Console.Write('@');
            else if (wallBlocks.Contains((i, j)))
                Console.Write('#');
            else if (boxBlocks.Contains((i, j)))
                Console.Write('[');
            else if (boxBlocks.Contains((i, j - 1)))
                Console.Write(']');
            else
                Console.Write('.');
        }
        Console.WriteLine("");
    }
}

void displayGrid(char[][] grid, int rows, int cols, int robotx, int roboty)
{
    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            if (i == roboty && j == robotx)
                Console.Write('@');
            else
                Console.Write(grid[i][j]);
        }
        Console.WriteLine("");
    }
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

// Now check and do push over a range of x values.
bool doPushUpDown(int dy, int xl, int xr, int y, char[][] grid)
{
    //Console.WriteLine($"doPushUpDown {dy} {xl} {xr} {y}");

    // we're free to move - do we need to extend the range?
    // do we need to widen our range a bit?
    // first time this will just be @ so should be ok.
    int nextXL = xl;
    int nextXR = xr;
    if (grid[y][xl] == ']') nextXL--;
    if (grid[y][xr] == '[') nextXR++;

    // if any of our lot is a wall then return false.
    for (int k = nextXL; k <= nextXR; k++)
    {
        if (grid[y][k] != '.' && grid[y+dy][k] == '#')
            return false;
    }

    bool moveNow = true;
    for (int k = nextXL; k <= nextXR; k++)
    {
        moveNow = moveNow && (grid[y+dy][k] == '.' || (grid[y][k] == '.' && grid[y+dy][k] == '#'));

    }

    if (moveNow)
    {
        for (int k = nextXL; k <= nextXR; k++)
        {
            if (grid[y][k] != '.') grid[y + dy][k] = grid[y][k];
            grid[y][k] = '.';
        }
        return true;
    }

    if (doPushUpDown(dy, nextXL, nextXR, y + dy, grid))
    {
        // ok, we need to do some swapping now.
        for (int k = nextXL; k <= nextXR; k++)
        {
            if (grid[y][k] != '.') grid[y + dy][k] = grid[y][k];
            grid[y][k] = '.';
        }
        return true;
    }

    return false;
}

// this return (dx,dy)! Note.
(int, int) dirToChange(char c)
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

long lanternFishCalculation(char[][] grid, int rows, int cols, char c)
{
    long total = 0;
    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            if (grid[i][j] == c)
            {
                total += (100 * i) + j;
            }
        }
    }
    return total;
}

void Part2(string[] lines, List<string> directions, int rows, int cols)
{
    int robotx = 0;
    int roboty = 0;
    char[][] grid = new char[rows][];

    HashSet<(int, int)> wallBlocks = new();
    HashSet<(int, int)> boxBlocks = new();
    ExpandGrid(lines, rows, cols, ref robotx, ref roboty, wallBlocks, boxBlocks);
    cols = cols * 2;
    displayPosiGrid(wallBlocks, boxBlocks, robotx, roboty, cols, rows);

    foreach (var dirLine in directions)
    {
        for (int q = 0; q < dirLine.Length; q++)
        {
            Console.ReadKey();
            Console.Clear();
            displayPosiGrid(wallBlocks, boxBlocks, robotx, roboty, cols, rows);
            Console.WriteLine($"Next move {dirLine[q]}");
            (int dx, int dy) = dirToChange(dirLine[q]);

            if (dirLine[q] == '<' || dirLine[q] == '>')
            {
                robotx = HorizontalFlow(robotx, roboty, wallBlocks, boxBlocks, dx);
            }
            else
            {
                roboty = VerticalFlow(robotx, roboty, wallBlocks, boxBlocks, dy);
            }
            displayPosiGrid(wallBlocks, boxBlocks, robotx, roboty, cols, rows);

        }
    }

    var total = lanternFishCalculation(grid, rows, cols, '[');
    Console.WriteLine($"Total is {total}");

    int VerticalFlow(int robotx, int roboty, HashSet<(int, int)> wallBlocks, HashSet<(int, int)> boxBlocks, int dx)
    {
        return -1;
    }

    int HorizontalFlow(int robotx, int roboty, HashSet<(int, int)> wallBlocks, HashSet<(int, int)> boxBlocks, int dx)
    {
        int nextcol = robotx + dx;
        (int _, int boxstart) = nextPosForBoxHorizontal(nextcol, roboty, dx);
        HashSet<(int, int)> toMove = new();
        while (true)
        {
            if (wallBlocks.Contains((roboty, nextcol)))
                break; // we're done, move nothing.
            if (boxBlocks.Contains((roboty, boxstart))) // it's a box
            {
                toMove.Add((roboty, boxstart));
                (int _, boxstart) = nextPosToSearchAfterBoxHorizontal(boxstart + dx, roboty, dx);
                nextcol = (dx == 1) ? boxstart : boxstart + 1; // (int _, boxstart) = nextPosForBoxHorizontal(nextcol, roboty, dx);
            }
            else // we've got a '.'
            {
                foreach (var mover in toMove)
                {
                    boxBlocks.Remove(mover);
                    boxBlocks.Add((mover.Item1, mover.Item2 + dx));
                }
                robotx += dx;
                break;
            }
        }

        return robotx;
    }
}

(int,int) nextPosForBoxHorizontal(int x, int y, int dx)
{
    if (dx == 1) return (y, x);
    else return (y, x - 1);
}

(int, int) nextPosToSearchAfterBoxHorizontal(int x, int y, int dx)
{
    if (dx == 1) return (y, x+1);
    else return (y, x - 1);
}

void Part1(string[] lines, List<string> directions, int rows, int cols)
{
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
                grid[i][j] = '.';
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

    var total = lanternFishCalculation(grid, rows, cols, 'O');
    Console.WriteLine($"Total is {total}");
}

