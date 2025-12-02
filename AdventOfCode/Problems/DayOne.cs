namespace AdventOfCode.Problems;

public class DayOne : Problem<int>
{
    public override int Day => 1;

    public override async Task<int> SolvePartOneAsync(IAsyncEnumerable<string?> lines)
    {
        const int startPosition = 50;
        int currentPosition = startPosition;
        int zeroPositionCount = 0;
        await foreach (string? line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                break;
            }

            DialMovementInstruction instruction = DialMovementInstruction.Parse(line);

            currentPosition += instruction.Direction == DialMovementDirection.Left
                ? -instruction.Steps
                : instruction.Steps;

            // Ensure the position wraps around a dial of size 100
            currentPosition = (currentPosition + 100) % 100;

            Console.WriteLine($"Moved {instruction.Direction} by {instruction.Steps} steps to position {currentPosition}");
            if (currentPosition == 0)
            {
                zeroPositionCount++;
            }
        }

        return zeroPositionCount;
    }

    public override async Task<int> SolvePartTwoAsync(IAsyncEnumerable<string?> lines)
    {
        const int startPosition = 50;
        int currentPosition = startPosition;
        int password = 0;

        await foreach (string? line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                break;
            }

            DialMovementInstruction instruction = DialMovementInstruction.Parse(line);
            int clicks = instruction.Steps;
            int laps = 0;

            if (instruction.Direction == DialMovementDirection.Right)
            {
                // Right movement: count complete wraps (each wrap passes through 0)
                laps = (currentPosition + clicks) / 100;
                currentPosition = (currentPosition + clicks) % 100;
            }
            else
            {
                // Left movement: more complex due to negative handling
                if (currentPosition == 0)
                {
                    // Starting at 0: only every full 100 steps hits 0
                    laps = clicks / 100;
                }
                else
                {
                    // Not at 0: check if we go past it
                    if (clicks < currentPosition)
                    {
                        laps = 0;
                    }
                    else
                    {
                        laps = 1 + ((clicks - currentPosition) / 100);
                    }
                }
                // Handle negative modulo correctly
                currentPosition = (currentPosition - (clicks % 100) + 100) % 100;
            }

            password += laps;
            Console.WriteLine($"Moved {instruction.Direction} by {clicks} steps to position {currentPosition}, laps: {laps}");
        }

        return password;
    }
}

internal record DialMovementInstruction(DialMovementDirection Direction, int Steps)
{
    public static DialMovementInstruction Parse(string line)
    {
        char direction = line[0];
        if (!int.TryParse(line[1..], out int steps))
        {
            throw new InvalidOperationException($"Invalid steps in instruction: {line}");
        }

        return new DialMovementInstruction(direction switch
        {
            'L' => DialMovementDirection.Left,
            'R' => DialMovementDirection.Right,
            _ => throw new InvalidOperationException($"Invalid direction character: {direction}")
        }, steps);
    }
}

internal enum DialMovementDirection : byte
{
    Left = 0,
    Right = 1
}
