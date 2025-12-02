using AdventOfCode.Problems;

namespace AdventOfCode.Tests.Problems;

public class DayTwoTests
{
    private readonly DayTwo _dayTwo = new();

    [Theory]
    [InlineData(true, 1227775554)]
    [InlineData(false, 4174379265)]
    public async Task DayTwo_SampleInput_ReturnsExpectedResult(bool partOne, long expectedAnswer)
    {
        const string sampleInput = """
                                   11-22,95-115,998-1012,1188511880-1188511890,222220-222224,
                                   1698522-1698528,446443-446449,38593856-38593862,565653-565659,
                                   824824821-824824827,2121212118-2121212124
                                   """;

        long answer = partOne
            ? await _dayTwo.SolvePartOneAsync(sampleInput.ReplaceLineEndings("").TextToAsyncLines())
            : await _dayTwo.SolvePartTwoAsync(sampleInput.ReplaceLineEndings("").TextToAsyncLines());

        Assert.Equal(expectedAnswer, answer);
    }
}
