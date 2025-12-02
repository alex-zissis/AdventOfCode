using AdventOfCode.Problems;

namespace AdventOfCode.Tests.Problems;

public class UnitTest1
{
    private readonly DayOne _solver = new();

    [Theory]
    [InlineData(true, 3)]
    [InlineData(false, 6)]
    public async Task DayOne_PartOne_WhenGivenSampleInput_ReturnsCorrectResult(bool partOne, int expectedAnswer)
    {
        const string sampleInput = """
        L68
        L30
        R48
        L5
        R60
        L55
        L1
        L99
        R14
        L82
        """;

        int answer = partOne
            ? await _solver.SolvePartOneAsync(sampleInput.TextToAsyncLines())
            : await _solver.SolvePartTwoAsync(sampleInput.TextToAsyncLines());

        Assert.Equal(expectedAnswer, answer);
    }

    [Theory]
    [InlineData("L50\nR50")]
    [InlineData("R50\nL50")]
    public async Task DayOne_PartOne_WhenGivenLoopbackValues_ReturnsCorrectResult(string input)
    {
        int answer = await _solver.SolvePartOneAsync(input.TextToAsyncLines());

        Assert.Equal(1, answer);
    }

    [Theory]
    [InlineData("L500")]
    public async Task DayOne_PartTwo_WhenPassingZeroMultipleTimes_GivesCorrectValue(string input)
    {
        int answer = await _solver.SolvePartTwoAsync(input.TextToAsyncLines());

        Assert.Equal(5, answer);
    }
}
