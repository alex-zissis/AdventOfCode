using AdventOfCode.Utility;

namespace AdventOfCode.Problems;

public class Day2 : Problem<long>
{
    public override int Day => 2;

    public override async Task<long> SolvePartOneAsync(IAsyncEnumerable<string?> lines)
    {
        string line = await lines.GetSingleLineAsync();

        long invalidNumberSum = 0;

        foreach ((long lowerBound, long upperBound) in ParseRanges(line))
        {
            for (long i = lowerBound; i <= upperBound; i++)
            {
                if (IsMadeUpEntirelyOfSingleRepeatingSequence(i))
                {
                    invalidNumberSum += i;
                }
            }
        }

        return invalidNumberSum;
    }

    public override async Task<long> SolvePartTwoAsync(IAsyncEnumerable<string?> lines)
    {
        string line = await lines.GetSingleLineAsync();

        long invalidNumberSum = 0;

        foreach ((long lowerBound, long upperBound) in ParseRanges(line))
        {
            for (long i = lowerBound; i <= upperBound; i++)
            {
                if (IsMadeUpEntirelyOfRepeatingSequenceOfAnyLength(i))
                {
                    Console.WriteLine("Invalid ID found: " + i);
                    invalidNumberSum += i;
                }
            }
        }

        return invalidNumberSum;
    }

    /// <summary>
    /// Is the number composed of a single repeating sequence.
    /// Given a string of length N, a sequence is length N/2 that repeats twice.
    /// </summary>
    private bool IsMadeUpEntirelyOfSingleRepeatingSequence(long number)
    {
        string numberStr = number.ToString();
        if (numberStr.Length % 2 != 0)
        {
            return false;
        }

        return numberStr[..(numberStr.Length / 2)] == numberStr[(numberStr.Length / 2)..];
    }

    /// <summary>
    /// Is the number composed entirely of any repeating sequence.
    /// </summary>
    private bool IsMadeUpEntirelyOfRepeatingSequenceOfAnyLength(long number)
    {
        string numberStr = number.ToString();
        int longestSequenceLength = numberStr.Length / 2;

        for (int i = longestSequenceLength; i > 0; i--)
        {
            string?[] chunks = numberStr.Chunk(i).Select(c => new string(c)).ToArray();

            if (chunks.All(chunk => chunk == chunks[0]))
            {
                return true;
            }
        }

        return false;
    }

    IEnumerable<(long LowerBound, long UpperBound)> ParseRanges(string line)
    {
        foreach (string range in line.Split(',', StringSplitOptions.TrimEntries))
        {
            string[] bounds = range.Split('-', StringSplitOptions.TrimEntries);
            long start = long.Parse(bounds[0]);
            long end = long.Parse(bounds[1]);

            yield return (start, end);
        }
    }
}
