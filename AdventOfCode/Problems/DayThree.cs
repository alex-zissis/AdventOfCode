using System.Text;

namespace AdventOfCode.Problems;

public sealed class DayThree : Problem<long>
{
    public override int Day => 3;

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
            
            power += GetLargetNDigitNumber(banks, banks, [], n);
        }

        return power;
    }

    internal static long GetLargetNDigitNumber(Bank[] banks, Bank[] remainingBanks, List<Bank> selectedBanks, int n)
    {
        if (n == 0)
        {
            var sb = new StringBuilder();
            foreach (var bank in selectedBanks.OrderBy(b => b.Index))
            {
                sb.Append(bank.Power);
            }

            Console.WriteLine("Line: " + string.Join("", selectedBanks.Select(b => b.Power)) + " : result = " + sb.ToString());
            return long.Parse(sb.ToString());
        }

        if (remainingBanks.Length == 0)
        {
            throw new InvalidOperationException("Not enough banks to select from");
        }

        if (remainingBanks.Length < n)
        {
            throw new InvalidOperationException("Not enough banks to select from for the remaining digits. Remaining: " + n + ", available: " + remainingBanks.Length + ". Banks: " + string.Join("", banks.Select(b => b.Power)));
        }

        var candidates = remainingBanks.Take(remainingBanks.Length - n + 1).ToArray();
        Console.WriteLine("Candidates for position " + (selectedBanks.Count + 1) + ": " + string.Join("", candidates.Select(b => b.Power)));

        // we need to find the largest digit that has enough digits after it
        // When there are multiple occurrences of the highest digit, pick the leftmost one
        Bank? highestBank = null;

        for (int i = 0; i < candidates.Length; i++)
        {
            if (highestBank is null || candidates[i].Power > highestBank.Value.Power)
            {
                highestBank = candidates[i];
            }
        }

        Console.WriteLine("Input banks: " + string.Join("", banks.Select(b => b.Power)));
        Console.WriteLine("Selected bank: " + highestBank?.Power);

        if (highestBank is null)
        {
            throw new InvalidOperationException("No highest bank found");
        }

        selectedBanks.Add(highestBank.Value);
        return GetLargetNDigitNumber(banks, [.. banks.Skip(highestBank.Value.Index + 1)], selectedBanks, n - 1);
    }

    internal static Bank[] ParseBanks(string line)
    {
        return [.. line.Select((c, i) => new Bank(i, int.Parse(c.ToString())))];
    }

    internal record struct Bank(int Index, int Power);
}
