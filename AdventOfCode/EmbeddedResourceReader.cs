using System.Reflection;

namespace AdventOfCode;

public static class EmbeddedResourceReader
{
    public static async Task<string> ReadAsync(string resourceName)
    {
        await using Stream? stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream(NormalizeResourceName(resourceName));

        if (stream is null)
        {
            throw new InvalidOperationException($"Could not find embedded resource {resourceName} ({NormalizeResourceName(resourceName)}). Available resources: {string.Join(", ", Assembly.GetExecutingAssembly().GetManifestResourceNames())}");
        }

        using StreamReader reader = new(stream);
        return await reader.ReadToEndAsync();
    }

    public static async IAsyncEnumerable<string?> ReadAllLinesAsync(string resourceName)
    {
        await using Stream? stream = Assembly.GetExecutingAssembly()
            .GetManifestResourceStream(NormalizeResourceName(resourceName));

        if (stream is null)
        {
            throw new InvalidOperationException($"Could not find embedded resource {resourceName} ({NormalizeResourceName(resourceName)}). Available resources: {string.Join(", ", Assembly.GetExecutingAssembly().GetManifestResourceNames())}");
        }

        using StreamReader reader = new(stream);
        string? line;
        while ((line = await reader.ReadLineAsync()) is not null)
        {
            yield return line;
        }
    }

    private static string NormalizeResourceName(string resourceName)
    {
        return "AdventOfCode.Resources." + resourceName.Replace("/", ".").Replace("\\", ".");
    }
}
