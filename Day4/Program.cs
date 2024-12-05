// See https://aka.ms/new-console-template for more information
using System.Drawing;
using System.Runtime.Serialization;
using System.Security.Cryptography.X509Certificates;

Console.WriteLine("Hello, World!");
var board = File.ReadAllLines(args[0]);
int ny = board.Length;
int nx = board[0].Length;

//
// For Part1 there are eight directional masks tested at each position. Each one should
// yield a string that is XMAS or not.
//
// For Part2 there is only one X shaped mask (I thought a cross shaped mask was required but
// obvioulsy not.) The mast gives two strings that are MAS or SAM - an easier one than
// the Part1 solution.
//

//Part1(board, ny, nx);
Part2(board, ny, nx);

static ((int, int), (int, int), (int, int), (int, int)) up(int x, int y, int nx, int ny)
{
    if (y - 3 < 0) throw new ArgumentException("no up");
    return ((x, y), (x, y - 1), (x, y - 2), (x, y - 3));
}

static ((int, int), (int, int), (int, int), (int, int)) left(int x, int y, int nx, int ny)
{
    if (x - 3 < 0) throw new ArgumentException("no up");
    return ((x, y), (x - 1, y), (x - 2, y), (x - 3, y));
}
static ((int, int), (int, int), (int, int), (int, int)) down(int x, int y, int nx, int ny)
{
    if (y + 3 >= ny) throw new ArgumentException("no up");
    return ((x, y), (x, y + 1), (x, y + 2), (x, y + 3));
}

static ((int, int), (int, int), (int, int), (int, int)) right(int x, int y, int nx, int ny)
{
    if (x + 3 >= nx) throw new ArgumentException("no up");
    return ((x, y), (x + 1, y), (x + 2, y), (x + 3, y));
}

static ((int, int), (int, int), (int, int), (int, int)) rightup(int x, int y, int nx, int ny)
{
    if (x + 3 >= nx || y - 3 < 0) throw new ArgumentException("no up");
    return ((x, y), (x + 1, y - 1), (x + 2, y - 2), (x + 3, y - 3));
}

static ((int, int), (int, int), (int, int), (int, int)) rightdown(int x, int y, int nx, int ny)
{
    if (x + 3 >= nx || y + 3 >= ny) throw new ArgumentException("no up");
    return ((x, y), (x + 1, y + 1), (x + 2, y + 2), (x + 3, y + 3));
}

static ((int, int), (int, int), (int, int), (int, int)) leftup(int x, int y, int nx, int ny)
{
    if (x - 3 < 0 || y - 3 < 0) throw new ArgumentException("no up");
    return ((x, y), (x - 1, y - 1), (x - 2, y - 2), (x - 3, y - 3));
}

static ((int, int), (int, int), (int, int), (int, int)) leftdown(int x, int y, int nx, int ny)
{
    if (x - 3 < 0 || y + 3 >= ny) throw new ArgumentException("no up");
    return ((x, y), (x - 1, y + 1), (x - 2, y + 2), (x - 3, y + 3));
}

static bool Match(string m, string[] board, ((int, int), (int, int), (int, int), (int, int)) pos)
{
    ((int, int) p1, (int, int) p2, (int, int) p3, (int, int) p4) = pos;
    string cand = $"{board[p1.Item2][p1.Item1]}{board[p2.Item2][p2.Item1]}{board[p3.Item2][p3.Item1]}{board[p4.Item2][p4.Item1]}";
    return String.Equals(m, cand);
}

static void Part2(string[] board, int ny, int nx)
{
    int count = 0;

    for (int i = 0; i < ny; i++)
    {
        for (int j = 0; j < nx; j++)
        {
            /*try
            {
                var coord = CrossMask(j, i, nx, ny);
                var barw = Word(board, coord.Item1);
                var bard = Word(board, coord.Item2);
                if ((String.Equals(barw, "MAS") || String.Equals(barw, "SAM")) &&
                    (String.Equals(bard, "MAS") || String.Equals(bard, "SAM")))
                {
                    Console.WriteLine($"Cross mask at {j},{i}");
                    count++;
                }
            }
            catch { }*/
            
            try
            {
                var coord = XMask(j, i, nx, ny);
                var barw = Word(board, coord.Item1);
                var bard = Word(board, coord.Item2);
                if ((String.Equals(barw, "MAS") || String.Equals(barw, "SAM")) &&
                    (String.Equals(bard, "MAS") || String.Equals(bard, "SAM")))
                {
                    Console.WriteLine($"Xxxxx mask at {j} {i}");
                    count++;
                }
            }
            catch { } 
        }
    }
    Console.WriteLine($"Total = {count}");
}

static string Word(string[] board, (Point,Point,Point) pos)
{
    (int bx1, int by1) = pos.Item1;
    (int bx2, int by2) = pos.Item2;
    (int bx3, int by3) = pos.Item3;
    return $"{board[by1][bx1]}{board[by2][bx2]}{board[by3][bx3]}";
}

static ((Point, Point, Point), (Point, Point, Point)) CrossMask(int x, int y, int nx, int ny)
{
    if (x - 1 < 0 || x + 1 >= nx || y - 1 < 0 || y + 1 >= ny) throw new ArgumentException("nope!");
    return (
        (new Point(x - 1, y), new Point(x, y), new Point(x + 1, y)),
        (new Point(x, y + 1), new Point(x, y), new Point(x, y - 1))
        );
}

static ((Point, Point, Point), (Point, Point, Point)) XMask(int x, int y, int nx, int ny)
{
    if (x - 1 < 0 || x + 1 >= nx || y - 1 < 0 || y + 1 >= ny) throw new ArgumentException("nope!");
    return (
        (new Point(x-1, y-1),new Point(x,y), new Point(x+1,y+1)),
        (new Point(x-1, y+1),new Point(x,y), new Point(x+1,y-1))
        );
}

static void Part1(string[] board, int ny, int nx)
{
    int count = 0;

    for (int i = 0; i < ny; i++)
    {
        for (int j = 0; j < nx; j++)
        {
            try
            {
                var coordup = up(j, i, nx, ny);
                if (Match("XMAS", board, coordup))
                {
                    Console.WriteLine($"UPward XMAS at ({j},{i})");
                    count++;
                }
            }
            catch
            {
            }

            try
            {
                var coord = down(j, i, nx, ny);
                if (Match("XMAS", board, coord))
                {
                    Console.WriteLine($"DOWNward XMAS at ({j},{i})");
                    count++;
                }
            }
            catch
            {
            }
            try
            {
                var coord = left(j, i, nx, ny);
                if (Match("XMAS", board, coord))
                {
                    Console.WriteLine($"LEFTward XMAS at ({j},{i})");
                    count++;
                }
            }
            catch
            {
            }
            try
            {
                var coord = right(j, i, nx, ny);
                if (Match("XMAS", board, coord))
                {
                    Console.WriteLine($"RIGHTward XMAS at ({j},{i})");
                    count++;
                }
            }
            catch
            {
            }
            try
            {
                var coord = leftdown(j, i, nx, ny);
                if (Match("XMAS", board, coord))
                {
                    Console.WriteLine($"LEFTDOWNward XMAS at ({j},{i})");
                    count++;
                }
            }
            catch
            {
            }
            try
            {
                var coord = leftup(j, i, nx, ny);
                if (Match("XMAS", board, coord))
                {
                    Console.WriteLine($"LEFTUPward XMAS at ({j},{i})");
                    count++;
                }
            }
            catch
            {
            }
            try
            {
                var coord = rightdown(j, i, nx, ny);
                if (Match("XMAS", board, coord))
                {
                    Console.WriteLine($"RIGHTDOWNward XMAS at ({j},{i})");
                    count++;
                }
            }
            catch
            {
            }
            try
            {
                var coord = rightup(j, i, nx, ny);
                if (Match("XMAS", board, coord))
                {
                    Console.WriteLine($"RIGHTUPward XMAS at ({j},{i})");
                    count++;
                }
            }
            catch
            {
            }
        }
    }

    Console.WriteLine($"Found {count} matches");
}


public record Point(int x, int y)
{
    (int, int) Deconstruct() { return (x, y); }
}

