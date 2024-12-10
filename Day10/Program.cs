// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

var lines = File.ReadAllLines(args[0]);

int rows = lines.Length;
int cols = lines[0].Length;

int[][] grid = new int[lines.Length][];
bool[][] visited = new bool[lines.Length][];
int[][] counts = new int[lines.Length][];
List<(int,int)> starts = new List<(int,int)> ();

for (int i = 0; i < lines.Length; i++)
{
    var line = lines[i];
    grid[i] = new int[cols];
    visited[i] = new bool[cols];
    counts[i] = new int[cols];
    for (int j = 0; j < cols; j++)
    {
        grid[i][j] = lines[i][j] - '0';
        visited[i][j] = false;
        counts[i][j] = 0;
        if (grid[i][j] == 0)
            starts.Add((i, j));
    }
}

Console.WriteLine($"OK - {rows} rows, {cols} cols, {starts.Count} starts");
Dictionary<(int, int, int), HashSet<(int,int)>> memos = new Dictionary<(int, int, int), HashSet<(int,int)>>();
Dictionary<(int, int, int), int> memos2 = new();

int total = 0;
HashSet<(int, int)> all = new();
foreach (var start in starts)
{
    //    var val = letsGo(start.Item1, start.Item2, 0, grid, visited, counts, memos);
    //    Console.WriteLine($"Start {start.Item1}, {start.Item2} has {val.Count} heights it can reach.");
    //    foreach (var g in val) all.Add(g);
    //    total += val.Count;

    var val = letsGoPart2(start.Item1, start.Item2, 0, grid, visited, counts, memos2);
    Console.WriteLine($"Start {start.Item1}, {start.Item2} has {val} paths.");
    total += val;
}

Console.WriteLine($"{total} {all.Count}");

int niner = 0;
for (int i = 0; i < rows; i++)
    for (int j = 0; j < cols; j++)
    {
        if (grid[i][j] == 9)
            niner += counts[i][j];
    }
Console.WriteLine($"{niner}");

//
// With the memoization this one is just about counting paths. So only an int required.
//
static int letsGoPart2(int row, int col, int height, int[][] grid, bool[][] visited, int[][] counts, Dictionary<(int, int, int), int> memos)
{
    if (grid[row][col] != height)
    {
        //memos.Add((row, col, height), 0);
        return 0;
    }

    if (grid[row][col] == 9)
    {
        counts[row][col] += 1;
        visited[row][col] = true;
        //memos.Add((row, col, height), );
        return 1;
    }

    if (memos.ContainsKey((row, col, height)))
    {
        return memos[(row, col, height)];
    }

    int total = 0;
    if (row + 1 < grid.Length)
    {
        var mine = letsGoPart2(row + 1, col, height + 1, grid, visited, counts, memos);
        total += mine;
    }
    if (row - 1 >= 0)
    {
        var mine2 = letsGoPart2(row - 1, col, height + 1, grid, visited, counts, memos);
        total += mine2;
    }
    if (col + 1 < grid[0].Length)
    {
        var mine3 = letsGoPart2(row, col + 1, height + 1, grid, visited, counts, memos);
        total += mine3;
    }
    if (col - 1 >= 0)
    {
        var mine4 = letsGoPart2(row, col - 1, height + 1, grid, visited, counts, memos);
        total += mine4;
    }

    memos.Add((row, col, height), total);
    return total;
}

//
// This is all about unique 9's reached, hence the hash set.
//
static HashSet<(int, int)> letsGo(int row, int col, int height, int[][] grid, bool[][] visited, int[][] counts, Dictionary<(int, int, int), HashSet<(int, int)>> memos)
{
    if (grid[row][col] != height)
    {
        //memos.Add((row, col, height), 0);
        return new HashSet<(int, int)>();
    }

    if (grid[row][col] == 9)
    {
        counts[row][col] += 1;
        visited[row][col] = true;
        //memos.Add((row, col, height), );
        return new HashSet<(int, int)>() { (row, col) }; // counts[row][col];
    }

    if (memos.ContainsKey((row, col, height)))
    {
        return memos[(row, col, height)];
    }

    HashSet<(int, int)> sops = new();
    if (row + 1 < grid.Length)
    {
        var mine = letsGo(row + 1, col, height + 1, grid, visited, counts, memos);
        foreach (var p in mine)
            sops.Add(p);
    }
    if (row - 1 >= 0)
    {
        var mine2 = letsGo(row - 1, col, height + 1, grid, visited, counts, memos);
        foreach (var p in mine2)
            sops.Add(p);
    }
    if (col + 1 < grid[0].Length)
    {
        var mine3 = letsGo(row, col + 1, height + 1, grid, visited, counts, memos);
        foreach (var p in mine3)
            sops.Add(p);
    }
    if (col - 1 >= 0)
    {
        var mine4 = letsGo(row, col - 1, height + 1, grid, visited, counts, memos);
        foreach (var p in mine4)
            sops.Add(p);
    }

    memos.Add((row, col, height), sops);
    return sops;
}
