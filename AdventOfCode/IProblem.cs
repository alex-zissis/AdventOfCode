namespace AdventOfCode;

public interface IProblem
{
    Task<object?> SolvePartOneAsync(IAsyncEnumerable<string?> lines);
    Task<object?> SolvePartTwoAsync(IAsyncEnumerable<string?> lines);
}
