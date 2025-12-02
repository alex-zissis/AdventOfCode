namespace AdventOfCode;

public abstract class Problem<TAnswer>
{
    public abstract int Day { get; }

    public abstract Task<TAnswer> SolvePartOneAsync(IAsyncEnumerable<string?> lines);
    public abstract Task<TAnswer> SolvePartTwoAsync(IAsyncEnumerable<string?> lines);
}
