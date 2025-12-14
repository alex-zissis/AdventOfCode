using System.Text;

namespace AdventOfCode.Problems;

[Day(3)]
public sealed class Day3 : Problem<long>
{
    public override async Task<long> SolvePartOneAsync(IAsyncEnumerable<string?> lines)
    {
        return await SolveAsync(lines, 2);
    }

    public override async Task<long> SolvePartTwoAsync(IAsyncEnumerable<string?> lines)
    {
        return await SolveAsync(lines, 12);
    }

    private static async Task<long> SolveAsync(IAsyncEnumerable<string?> lines, int n)
    {
        long power = 0;

        await foreach (var line in lines)
        {
            if (string.IsNullOrEmpty(line))
            {
                continue;
            }

            Bank[] banks = ParseBanks(line);
            
            power += GetLargetNDigitNumber(banks, 0, [], n);
        }

        return power;
    }

    internal static long GetLargetNDigitNumber(Bank[] banks, int startIndex, List<Bank> selectedBanks, int n)
    {
        if (n == 0)
        {
            var sb = new StringBuilder();
            foreach (var bank in selectedBanks)
            {
                sb.Append(bank.Power);
            }

            Console.WriteLine("Line: " + string.Join("", selectedBanks.Select(b => b.Power)) + " : result = " + sb.ToString());
            return long.Parse(sb.ToString());
        }

        int remainingCount = banks.Length - startIndex;
        
        if (remainingCount == 0)
        {
            throw new InvalidOperationException("Not enough banks to select from");
        }

        if (remainingCount < n)
        {
            throw new InvalidOperationException("Not enough banks to select from for the remaining digits. Remaining: " + n + ", available: " + remainingCount + ". Banks: " + string.Join("", banks.Select(b => b.Power)));
        }

        int candidatesEnd = banks.Length - n + 1;
        Console.WriteLine("Candidates for position " + (selectedBanks.Count + 1) + ": " + string.Join("", banks.Skip(startIndex).Take(candidatesEnd - startIndex).Select(b => b.Power)));

        // we need to find the largest digit that has enough digits after it
        // When there are multiple occurrences of the highest digit, pick the leftmost one
        Bank? highestBank = null;

        for (int i = startIndex; i < candidatesEnd; i++)
        {
            if (highestBank is null || banks[i].Power > highestBank.Value.Power)
            {
                highestBank = banks[i];
            }
        }

        Console.WriteLine("Input banks: " + string.Join("", banks.Select(b => b.Power)));
        Console.WriteLine("Selected bank: " + highestBank?.Power);

        if (highestBank is null)
        {
            throw new InvalidOperationException("No highest bank found");
        }

        selectedBanks.Add(highestBank.Value);
        return GetLargetNDigitNumber(banks, highestBank.Value.Index + 1, selectedBanks, n - 1);
    }

    internal static Bank[] ParseBanks(string line)
    {
        return [.. line.Select((c, i) => new Bank(i, int.Parse(c.ToString())))];
    }

    internal record struct Bank(int Index, int Power);
}
