// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

int boardRows = 103;
int boardCols = 101;
int time = 100;

var lines = File.ReadAllLines(args[0]);
List<Robot> robots = new List<Robot>();
for (int i = 0; i < lines.Length; i++)
{
    var r = Robot.Parse(lines[i]);
    robots.Add(r);
}

int q1 = 0;
int q2 = 0;
int q3 = 0;
int q4 = 0;

int rowmid = boardRows / 2;
int colmid = boardCols / 2;

foreach (var robot in robots)
{
    Console.Write($"Robot {robot.x} {robot.y} moves. ");
    (int px, int py) = robot.positionAfterTime(time, boardRows, boardCols);
    Console.WriteLine($"Robot after 100 s = {px} {py}");
//    board[py][px] += 1;

    if (px < colmid && py < rowmid) q1++;
    if (px < colmid && py > rowmid) q2++;
    if (px > colmid && py < rowmid) q3++;
    if (px > colmid && py > rowmid) q4++;
}
Console.WriteLine($"Score = {q1 * q2 * q3 * q4}");

//
// The trick to Part2 is to find something, visually, that looks
// like it might be all the robots coming together, work out a
// period and then step through the periods to see if what you
// get is a christmas tree.
//
// Silly, stupidly time consuming, still quite fun.
//
for (int t = 0; t < 12000; t++)
{
    int[][] board = new int[boardRows][];
    for (int i = 0; i < boardRows; i++)
    {
        board[i] = new int[boardCols];
        for (int j = 0; j < boardCols; j++)
            board[i][j] = 0;
    }

    foreach (var robot in robots)
    {
        board[robot.y][robot.x] += 1;
    }

    if (t==0 || (t-69) % 101 == 0)
    {
        Console.WriteLine($"{t} Seconds::");
        for (int i = 0; i < boardRows; i++)
        {
            for (int j = 0; j < boardCols; j++)
                if (board[i][j] == 0)
                    Console.Write('.');
                else
                    Console.Write($"{board[i][j]}");
            Console.WriteLine("");
        }
        Console.WriteLine("");
        Console.ReadLine();
    }

    foreach (var robot in robots) robot.oneStep(boardRows, boardCols);
}

public class Robot
{
    public int x { get; set; }
    public int y { get; set; }
    public int vx { get; set; }
    public int vy { get; set; }

    public Robot(int cx, int cy, int cvx, int cvy)
    {
        this.x = cx;
        this.y = cy;
        this.vx = cvx;
        this.vy = cvy;
    }

    public static Robot Parse(string line)
    {
        var splits = line.Split(new char[] { ' ', ',', '=' }, StringSplitOptions.RemoveEmptyEntries);
        int px = int.Parse(splits[1]);
        int py = int.Parse(splits[2]);
        int dx = int.Parse(splits[4]);
        int dy = int.Parse(splits[5]);
        return new Robot(px, py, dx, dy);
    }

    public (int, int) positionAfterTime(int seconds, int boardRows, int boardCols)
    {
        // we'll rely on some repeat position, then use mod arithmetic.
        Dictionary<(int, int), int> posToTime = new();
        Dictionary<int, (int, int)> timeToPos = new();
        posToTime.Add((x, y), 0);
        timeToPos.Add(0, (x, y));
        int px = x;
        int py = y;
        int i = 1;
        for ( ; i <= seconds; i++)
        {
            px = (px + vx);
            if (px < 0) px = boardCols + px; // MOD DOESN'T WORK GOING NEGATIVE!!!
            if (px >= boardCols) px = px % boardCols; 
            py = (py + vy);
            if (py < 0) py = boardRows + py;
            if (py >= boardRows) py = py % boardRows;

            if (posToTime.ContainsKey((px, py)))
                break;
            posToTime.Add((px, py), i);
            timeToPos.Add(i, (px, py));

        }
        if (i == seconds + 1)
        {
            return (px, py);
        }

        // we'll have to mod it.
        int finalStep = seconds % i;
        return timeToPos[finalStep];
    }

    public void oneStep(int boardRows, int boardCols)
    {
        int px = (x + vx);
        if (px < 0) px = boardCols + px; // MOD DOESN'T WORK GOING NEGATIVE!!!
        if (px >= boardCols) px = px % boardCols;
        int py = (y + vy);
        if (py < 0) py = boardRows + py;
        if (py >= boardRows) py = py % boardRows;
        x = px; y = py;
    }

}
