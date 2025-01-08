using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Day20
{
    static class Part2
    {
        static HashSet<(int, int)> moves = new HashSet<(int, int)>() { (-1, 0), (1, 0), (0, -1), (0, 1) };

        public static void DoPart2(string[] grid)
        {
            int maxCheatLength = 20;
            int requiredSaving = 100;
            int rows = grid.Length;
            int cols = grid[0].Length;
            (int, int) start = (-1, -1);
            (int, int) end = (-1, -1);
            HashSet<(int, int)> allpoints = new();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    if (grid[i][j] == 'S') start = (i, j);
                    if (grid[i][j] == 'E') end = (i, j);
                    if (grid[i][j] != '#') allpoints.Add((i, j));
                }
            }

            var startDistances = ShortestPath(grid, rows, cols, start.Item1, start.Item2);
            var endDistances = ShortestPath(grid, rows, cols, end.Item1, end.Item2);

            if (startDistances[end] != endDistances[start])
            {
                Console.WriteLine("Mismatch...");
            }

            int shortestPath = startDistances[end];
            Console.WriteLine($"Shortest path is {shortestPath}");

            Dictionary<int, int> savingsCounts = new();
            foreach (var p in allpoints)
            {
                int toEnd = endDistances[p];
                for (int dist = 2; dist <= maxCheatLength; dist++)
                {
                    var checkpoints = AllPointsAtDistance(p, dist, grid, rows, cols);
                    //Console.WriteLine($"Points to check = {checkpoints.Count} at distance {dist}");
                    foreach (var sp in checkpoints)
                    {
                        (int sprow, int spcol) = sp;
                        if (grid[sprow][spcol] != '#')
                        {
                            int toStart = startDistances[sp];
                            int totalDist = dist + toEnd + toStart;
                            if (totalDist < shortestPath)
                            {
                                int saving = shortestPath - totalDist;
                                if (savingsCounts.ContainsKey(saving))
                                    savingsCounts[saving] += 1;
                                else 
                                    savingsCounts[saving] = 1;
                                //Console.WriteLine($"Found one! {saving}");
                            }
                        }
                    }
                }
            }
            int result = 0;

            foreach ((int k, int s) in savingsCounts)
            {
                if (k >= requiredSaving)
                {
                    result += s;
                    Console.WriteLine($"{k} picoseconds = {s}");
                }
            }
            Console.WriteLine($"{result} savings of at least 100 picoseconds.");
        }

        private static HashSet<(int,int)> AllPointsAtDistance((int, int) p, int dist, string[] grid, int rows, int cols)
        {
            HashSet<(int, int)> result = new();
            (int row, int col) = p;
            for (int i = row - dist; i <= row + dist; i++)
            {
                for (int j = col - dist; j <= col + dist; j++)
                {
                    if (i >= 0 && i < rows && j >= 0 && j < cols)
                    {
                        int pdist = Math.Abs(row - i) + Math.Abs(col - j);
                        if (pdist == dist)
                            result.Add((i, j));

                    }
                }
            }
            return result;
        }

        static Dictionary<(int, int), int> ShortestPath(string[] grid, int rows, int cols, int startrow, int startcol)
        {
            UpdatablePriorityQueue upq = new();
            Dictionary<(int, int), int> distance = new();
            Dictionary<(int, int), (int, int)> prev = new();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    (int nodrow, int nodcol) = (i,j);
                    distance[(i,j)] = int.MaxValue;
                    prev[(i,j)] = (i,j);
                    if (i == startrow && j == startcol)
                        upq.Enqueue((i, j), 0);
                    else
                        upq.Enqueue((i,j), int.MaxValue);
                }
            }

            var startNode = (startrow, startcol);
            distance[startNode] = 0;
            
            while (upq.Count() > 0)
            {
                ((int, int) u, int udistance) = upq.Dequeue();
                (int urow, int ucol) = u;

                // edges?
                HashSet<(int, int)> nedges = new();
                foreach ((int dr, int dc) in moves)
                {
                    if (urow + dr >= 0 && urow + dr < rows &&
                        ucol + dc >= 0 && ucol + dc < cols)
                    {
                        var gcr = grid[urow + dr][ucol + dc];
                        if (gcr != '#')
                            nedges.Add((urow + dr, ucol + dc));
                    }
                }

                foreach (var v in nedges)
                {
                    if (v == u) continue;
                    int alt = udistance + 1;
                    if (alt >= 0 && alt < distance[v])
                    {
                        if (alt < 0)
                            Console.WriteLine("ohdear");
                        (int vrow, int vcol) = v;
                        distance[v] = alt;
                        prev[v] = u;
                        upq.UpdatePriority((vrow, vcol), alt);
                    }
                }
            }
            return distance;
        }
    }
}
