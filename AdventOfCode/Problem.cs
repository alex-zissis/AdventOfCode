namespace AdventOfCode;

public abstract class Problem<TAnswer> : IProblem
{
    public abstract Task<TAnswer> SolvePartOneAsync(IAsyncEnumerable<string?> lines);
    public abstract Task<TAnswer> SolvePartTwoAsync(IAsyncEnumerable<string?> lines);

    async Task<object?> IProblem.SolvePartOneAsync(IAsyncEnumerable<string?> lines)
    {
        return await SolvePartOneAsync(lines);
    }

    async Task<object?> IProblem.SolvePartTwoAsync(IAsyncEnumerable<string?> lines)
    {
        return await SolvePartTwoAsync(lines);
    }
}
