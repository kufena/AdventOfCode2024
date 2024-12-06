// See https://aka.ms/new-console-template for more information
using System.Numerics;

Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);
int ny = lines.Length;
int nx = lines[0].Length;
bool[][] board = new bool[ny][];
bool[][] visited = new bool[ny][];
int posx = 0;
int posy = 0;
int dir = 0;

for (int i = 0; i < ny; i++)
{
    board[i] = new bool[nx];
    visited[i] = new bool[nx];
    for (int j = 0; j < nx; j++)
    {
        visited[i][j] = false;
        board[i][j] = lines[i][j] != '#';
        if (lines[i][j] == '^')
        {
            posx = j; posy = i;
            visited[i][j] = true;
        }
    }
}

Part2(ny, nx, board, visited, ref posx, ref posy);
//Part1(ny, nx, board, visited, ref posx, ref posy, ref dir);

static (int, int) nextPost(int posx, int posy, int dir)
{
    if (dir == 0) return (posx, posy - 1);
    if (dir == 1) return (posx + 1, posy);
    if (dir == 2) return (posx, posy + 1);
    if (dir == 3) return (posx - 1, posy);
    throw new ArgumentException($"unknown direction {dir}");
}

static void Part2(int ny, int nx, bool[][] startboard, bool[][] visited, ref int posx, ref int posy)
{
    // follow the pointer until it goes off the board.
    // but remember visitations and directions and determine if you spot a cycle.

    int startx = posx;
    int starty = posy;
    int dir = 0;
    int obs = 0;

    for (int ally = 0; ally < ny; ally++)
    {
        for (int allx = 0; allx < nx; allx++)
        {
            dir = 0;
            if (allx == startx && ally == starty)
                continue; // miss out this one.
            if (!startboard[ally][allx])
                continue; // already an obstruction here.

            bool[][] board = new bool[ny][];
            for (int k = 0; k < ny; k++)
            {
                board[k] = new bool[nx];
                for (int l = 0; l < nx; l++)
                {
                    board[k][l] = startboard[k][l];
                    visited[k][l] = false;
                    if (k == ally && l == allx)
                        board[k][l] = false; // place obstacle here 
                }
            }
            HashSet<int>[][] directionsboard = new HashSet<int>[ny][];
            for (int i = 0; i < ny; i++)
            {
                directionsboard[i] = new HashSet<int>[nx];
                for (int j = 0; j < nx; j++)
                {
                    directionsboard[i][j] = new HashSet<int>();
                }
            }
            directionsboard[starty][startx].Add(dir);
            int count = 0;
            posx = startx;
            posy = starty;
            while (true)
            {
                (int nextx, int nexty) = nextPost(posx, posy, dir);
                if (nextx < 0 || nexty < 0 || nextx >= nx || nexty >= ny)
                    break; // we're off the board.
                if (board[nexty][nextx]) // we're good to move here.
                {
                    posx = nextx; posy = nexty;
                    visited[nexty][nextx] = true;
                    directionsboard[nexty][nextx].Add(dir);
                    count++;
                }
                else
                {
                    dir = (dir + 1) % 4;
                    if (visited[posy][posx] && directionsboard[posy][posx].Contains(dir))
                    {
                        obs++;
                        Console.WriteLine($"Obstacle at {allx} {ally}");
                        break;
                    }
                    directionsboard[posy][posx].Add(dir);
                }
            }
        }
    }
    Console.WriteLine($"We could introduce {obs} obstacles to make cycles.");
}

static void Part1(int ny, int nx, bool[][] board, bool[][] visited, ref int posx, ref int posy, ref int dir)
{
    // follow the pointer until it goes off the board.
    int count = 0;
    while (true)
    {
        (int nextx, int nexty) = nextPost(posx, posy, dir);
        if (nextx < 0 || nexty < 0 || nextx >= nx || nexty >= ny)
            break; // we're off the board.
        if (board[nexty][nextx]) // we're good to move here.
        {
            posx = nextx; posy = nexty;
            visited[nexty][nextx] = true;
            count++;
        }
        else
        {
            dir = (dir + 1) % 4;
        }
    }

    Console.WriteLine($"We visited {count} board squares.");
    count = 0;
    for (int i = 0; i < ny; i++)
    {
        for (int j = 0; j < nx; j++)
        {
            count += (visited[i][j] ? 1 : 0);
        }
    }
    Console.WriteLine($"Marked {count} distinct positions as visited.");
}
