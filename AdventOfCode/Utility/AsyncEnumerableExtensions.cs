namespace AdventOfCode.Utility;

public static class AsyncEnumerableExtensions
{
    public static async Task<string> GetSingleLineAsync(this IAsyncEnumerable<string?> lines)
    {
        await foreach(string? line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                throw new InvalidOperationException("No lines found.");
            }

            return line;
        }

        throw new InvalidOperationException("No lines found.");
    }
}
