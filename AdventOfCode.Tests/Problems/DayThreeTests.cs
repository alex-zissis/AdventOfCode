using System;
using AdventOfCode.Problems;

namespace AdventOfCode.Tests.Problems;

public sealed class DayThreeTests
{
    private readonly DayThree _solver = new();

    private const string SampleInput = """
        987654321111111
        811111111111119
        234234234234278
        818181911112111
        """;

    [Fact]
    public async Task DayThree_PartOne_WhenGivenSampleInput_ReturnsCorrectResult()
    {
        // Act
        long result = await _solver.SolvePartOneAsync(SampleInput.TextToAsyncLines());

        // Assert
        Assert.Equal(357, result);
    }

    [Fact]
    public async Task DayThree_PartTwo_WhenGivenSampleInput_ReturnsCorrectResult()
    {
        // Act
        long result = await _solver.SolvePartTwoAsync(SampleInput.TextToAsyncLines());

        // Assert
        Assert.Equal(3121910778619, result);
    }

    [Theory]
    [InlineData("987654321111111", 987654321111)]
    [InlineData("811111111111119", 811111111119)]
    [InlineData("234234234234278", 434234234278)]
    [InlineData("818181911112111", 888911112111)]
    public void DayThree_GetLargetNDigitNumber_WhenGivenLine_ReturnsExpectedResult(string line, long expected)
    {
        DayThree.Bank[] banks = DayThree.ParseBanks(line);

        long result = DayThree.GetLargetNDigitNumber(banks, 0, [], 12);

        Assert.Equal(expected, result);
    }
}
