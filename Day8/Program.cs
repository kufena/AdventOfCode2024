// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");
var lines = File.ReadAllLines(args[0]);
int rows = lines.Length;
int cols = lines[0].Length;

Dictionary<char, List<(int, int)>> antennas = new();
HashSet<(int, int)> uniqueAntennas = new();
for (int row = 0; row < rows; row++)
{
    for (int col = 0; col < cols; col++)
    {
        if (lines[row][col] != '.')
        {
            char ant = (char)lines[row][col];
            if (antennas.ContainsKey(ant))
            {
                antennas[ant].Add((col, row));
            }
            else
            {
                antennas.Add(ant, new List<(int, int)>() { (col, row) });
            }
            uniqueAntennas.Add((col, row));
        }
    }
}

Console.WriteLine($"{antennas.Keys.Count} antennas");

// Part1(rows, cols, antennas);
Part2(rows, cols, antennas);

static void Part2(int rows, int cols, Dictionary<char, List<(int, int)>> antennas)
{
    var antinodes = new Dictionary<char, HashSet<(int, int)>>();

    foreach (var ant in antennas.Keys)
    {
        antinodes.Add(ant, new());
        var positions = antennas[ant];
        foreach ((int c1, int r1) in positions)
        {
            foreach ((int c2, int r2) in positions)
            {
                if (c1 == c2 && r1 == r2)
                    continue;
                int coldiff = c1 - c2;
                int rowdiff = r1 - r2;

                int anticol = 0;
                int antirow = 0;
                anticol = c1 + coldiff; // (coldiff > 0) ? c2 + coldiff : (coldiff < 0 ? c1 + coldiff : -1);
                antirow = r1 + rowdiff; // (rowdiff > 0) ? r2 + rowdiff : (rowdiff < 0 ? r1 + rowdiff : -1);
                antinodes[ant].Add((c1, r1));
                antinodes[ant].Add((c2, r2));
                while (anticol >= 0 && anticol < cols && antirow >= 0 && antirow < rows) // && !uniqueAntennas.Contains((anticol,antirow)))
                {
                    antinodes[ant].Add((anticol, antirow));
                    anticol += coldiff;
                    antirow += rowdiff;
                }
            }
        }
    }

    HashSet<(int, int)> uniquePositions = new();
    foreach (var key in antinodes.Keys)
    {
        Console.WriteLine($"{key} has {antinodes[key].Count} antinodes.");
        foreach (var pair in antinodes[key])
        {
            Console.WriteLine($"{key} antinode at {pair.Item1} {pair.Item2}");
            uniquePositions.Add(pair);
        }
    }
    Console.WriteLine($"Total of {uniquePositions.Count} unique antinode squares.");
}

static void Part1(int rows, int cols, Dictionary<char, List<(int, int)>> antennas)
{
    var antinodes = new Dictionary<char, HashSet<(int, int)>>();

    foreach (var ant in antennas.Keys)
    {
        antinodes.Add(ant, new());
        var positions = antennas[ant];
        foreach ((int c1, int r1) in positions)
        {
            foreach ((int c2, int r2) in positions)
            {
                if (c1 == c2 && r1 == r2)
                    continue;
                int coldiff = c1 - c2;
                int rowdiff = r1 - r2;

                int anticol = 0;
                int antirow = 0;
                anticol = c1 + coldiff; // (coldiff > 0) ? c2 + coldiff : (coldiff < 0 ? c1 + coldiff : -1);
                antirow = r1 + rowdiff; // (rowdiff > 0) ? r2 + rowdiff : (rowdiff < 0 ? r1 + rowdiff : -1);

                if (anticol >= 0 && anticol < cols && antirow >= 0 && antirow < rows) // && !uniqueAntennas.Contains((anticol,antirow)))
                {
                    antinodes[ant].Add((anticol, antirow));
                }
            }
        }
    }

    HashSet<(int, int)> uniquePositions = new();
    foreach (var key in antinodes.Keys)
    {
        Console.WriteLine($"{key} has {antinodes[key].Count} antinodes.");
        foreach (var pair in antinodes[key])
        {
            Console.WriteLine($"{key} antinode at {pair.Item1} {pair.Item2}");
            uniquePositions.Add(pair);
        }
    }
    Console.WriteLine($"Total of {uniquePositions.Count} unique antinode squares.");
}
