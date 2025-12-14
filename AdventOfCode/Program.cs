using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AdventOfCode;

Dictionary<int, Type> problemsByDay = Assembly.GetExecutingAssembly()
    .GetTypes()
    .Where(type => !type.IsAbstract && IsProblemType(type))
    .Select(type => (Type: type, Attribute: type.GetCustomAttribute<DayAttribute>()))
    .Where(tuple => tuple.Attribute is not null)
    .ToDictionary(tuple => tuple.Attribute!.Day, tuple => tuple.Type);

RootCommand rootCommand = new("Advent of Code Solver");

Argument<int> dayArgument = new("day")
{
    Description = "Day of the problem to solve (e.g., 1 for Day One)."
};

Option<int> partOption = new("part", "--part", "-p")
{
    Required = false,
    DefaultValueFactory = _ => 1,
    Description = "Part of the problem to solve (1 or 2). Defaults to 1."
};

rootCommand.Arguments.Add(dayArgument);
rootCommand.Options.Add(partOption);

rootCommand.SetAction(RunAsync);

await rootCommand.Parse(args).InvokeAsync();

async Task RunAsync(ParseResult parseResult)
{
    int day = parseResult.GetRequiredValue(dayArgument);
    int part = parseResult.GetValue(partOption);

    if (part is < 1 or > 2)
    {
        Console.Error.WriteLine("Part must be 1 or 2.");
        return;
    }

    if (!TryResolveProblem(day, out IProblem? problem))
    {
        return;
    }

    IAsyncEnumerable<string?> lines = EmbeddedResourceReader.ReadAllLinesAsync($"Day_{day}.txt");

    object? answer = part == 1
        ? await problem.SolvePartOneAsync(lines)
        : await problem.SolvePartTwoAsync(lines);

    Console.WriteLine($"Day {day} part {part} answer: {answer}");
}

/// <summary>
/// Resolves the problem instance for the given day.
/// </summary>
bool TryResolveProblem(int day, [NotNullWhen(true)] out IProblem? problem)
{
    problem = null;

    if (!problemsByDay.TryGetValue(day, out Type? problemType))
    {
        Console.Error.WriteLine($"Problem for day {day} not found.");
        return false;
    }

    if (Activator.CreateInstance(problemType) is not IProblem instance)
    {
        Console.Error.WriteLine($"Failed to create instance of {problemType.Name}.");
        return false;
    }

    problem = instance;
    return true;
}

static bool IsProblemType(Type? type)
{
    while (type is not null && type != typeof(object))
    {
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Problem<>))
        {
            return true;
        }

        type = type.BaseType;
    }

    return false;
}
