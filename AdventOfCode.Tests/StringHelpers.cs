namespace AdventOfCode.Tests;

public static class StringHelpers
{
    public static async IAsyncEnumerable<string?> TextToAsyncLines(this string text)
    {
        string[] lines = text.Split(["\r\n", "\r", "\n"], StringSplitOptions.None);

        foreach (string line in lines)
        {
            yield return line;
        }
    }
}
