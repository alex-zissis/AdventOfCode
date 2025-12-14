using AdventOfCode.Problems;

namespace AdventOfCode.Tests.Problems;

public class Day4Tests
{
    private readonly Day4 _solver = new();

    private const string SampleInput = """
    ..@@.@@@@.
    @@@.@.@.@@
    @@@@@.@.@@
    @.@@@@..@.
    @@.@@@@.@@
    .@@@@@@@.@
    .@.@.@.@@@
    @.@@@.@@@@
    .@@@@@@@@.
    @.@.@@@.@.
    """;

    [Fact]
    public async Task Day4_PartOne_WhenGivenSampleInput_ReturnsCorrectResult()
    {
        // Act
        int result = await _solver.SolvePartOneAsync(SampleInput.TextToAsyncLines());

        // Assert
        Assert.Equal(13, result);
    }

    [Fact]
    public async Task Day4_PartTwo_WhenGivenSampleInput_ReturnsCorrectResult()
    {
        // Act
        int result = await _solver.SolvePartTwoAsync(SampleInput.TextToAsyncLines());

        // Assert
        Assert.Equal(43, result);
    }
}