namespace AdventOfCode.Problems;

[Day(4)]
public sealed class Day4 : Problem<int>
{
    public override async Task<int> SolvePartOneAsync(IAsyncEnumerable<string?> lines)
    {
        int candidate = 0;

        bool[][] grid = await ConstructGridAsync(lines);
        for (int row = 0; row < grid.Length; row++)
        {
            for (int col = 0; col < grid[row].Length; col++)
            {
                if (!grid[row][col])
                {
                    continue;
                }

                // check the 8 adjacent cells
                int adjacentCount = 0;
                ForEachNeighbor(row, col, (newRow, newCol) =>
                {
                    if (IsMarked(grid, newRow, newCol))
                    {
                        adjacentCount++;
                    }
                });
                

                if (adjacentCount < 4)
                {
                    candidate++;
                }
            }
        }

        return candidate;
    }

    public override async Task<int> SolvePartTwoAsync(IAsyncEnumerable<string?> lines)
    {
        // what makes a cell adjacent?
        // it is row - 1, row, or row + 1
        // and col - 1, col, or col + 1

        // first construct a grid
        // find removal candidates (part 1 logic)
        // remove them one by one
        // when removing, re-check neighbors to see if they are now candidates
        var visited = new HashSet<Cell>();

        // Implementation for Part Two
        int result = 0;
        bool[][] grid = await ConstructGridAsync(lines);
        var toProcess = new Queue<Cell>();

        for (int row = 0; row < grid.Length; row++)
        {
            for (int col = 0; col < grid[row].Length; col++)
            {
                if (!grid[row][col])
                {
                    continue;
                }

                // check the 8 adjacent cells
                int adjacentCount = 0;
                ForEachNeighbor(row, col, (newRow, newCol) =>
                {
                    if (IsMarked(grid, newRow, newCol))
                    {
                        adjacentCount++;
                    }
                });

                if (adjacentCount < 4)
                {
                    toProcess.Enqueue(new Cell(row, col));
                }
            }
        }

        while (toProcess.Count > 0)
        {
            Cell cell = toProcess.Dequeue();
            if (visited.Contains(cell) || !grid[cell.Row][cell.Col])
            {
                continue;
            }

            // Remove the cell
            grid[cell.Row][cell.Col] = false;
            visited.Add(cell);
            result++;

            // Re-evaluate neighbors
            ForEachNeighbor(cell.Row, cell.Col, (newRow, newCol) =>
            {
                if (IsMarked(grid, newRow, newCol))
                {
                    int adjacentCount = 0;
                    ForEachNeighbor(newRow, newCol, (adjRow, adjCol) =>
                    {
                        if (IsMarked(grid, adjRow, adjCol))
                        {
                            adjacentCount++;
                        }
                    });

                    if (adjacentCount < 4)
                    {
                        toProcess.Enqueue(new Cell(newRow, newCol));
                    }
                }
            });
        }

        return result;
    }

    private static void ForEachNeighbor(int row, int col, Action<int, int> action)
    {
        for (int deltaRow = -1; deltaRow <= 1; deltaRow++)
        {
            for (int deltaCol = -1; deltaCol <= 1; deltaCol++)
            {
                if (deltaRow == 0 && deltaCol == 0)
                {
                    continue; // skip the cell itself
                }

                action(row + deltaRow, col + deltaCol);
            }
        }
    }

    private static async Task<bool[][]> ConstructGridAsync(IAsyncEnumerable<string?> lines)
    {
        return [.. (await lines.ToArrayAsync()).Select(line => line!.Select(c => c == '@').ToArray())];
    }

    private static bool IsMarked(bool[][] grid, int row, int col)
    {
        return row >= 0 && row < grid.Length && col >= 0 && col < grid[row].Length && grid[row][col];
    }

    private record struct Cell(int Row, int Col);
}