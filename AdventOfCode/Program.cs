using System.CommandLine;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using AdventOfCode;

Type[] problemTypes = [.. Assembly.GetExecutingAssembly()
    .GetTypes()
    .Where(type => !type.IsAbstract && IsProblemType(type))];

RootCommand rootCommand = new("Advent of Code Solver");

Option<int> dayOption = new("Day", "--day", "-d")
{
    Required = true,
    Description = "Day of the problem as a number (e.g., 1 for Day One)."
};

Option<int> partOption = new("Part", "--part", "-p")
{
    Required = false,
    DefaultValueFactory = _ => 1,
    Description = "Part of the problem to solve (1 or 2). Defaults to 1."
};

rootCommand.Options.Add(dayOption);
rootCommand.Options.Add(partOption);

rootCommand.SetAction(RunAsync);

await rootCommand.Parse(args).InvokeAsync();

async Task RunAsync(ParseResult parseResult)
{
    int day = parseResult.GetRequiredValue(dayOption);
    int part = parseResult.GetValue(partOption);

    if (part is < 1 or > 2)
    {
        Console.Error.WriteLine("Part must be 1 or 2.");
        return;
    }

    if (!TryResolveProblem(day, out object? instance, out Type? problemType))
    {
        return;
    }

    string methodName = part == 1 ? nameof(Problem<>.SolvePartOneAsync) : nameof(Problem<>.SolvePartTwoAsync);
    MethodInfo? solver = problemType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    if (solver is null)
    {
        Console.Error.WriteLine($"{methodName} not found on {problemType.Name}.");
        return;
    }

    IAsyncEnumerable<string?> lines = EmbeddedResourceReader.ReadAllLinesAsync($"Day_{day}.txt");

    object? invocationResult = InvokeMethod(day, part, instance, problemType);
    object? answer = await GetAnswer(invocationResult);
   
    Console.WriteLine($"Day {day} part {part} answer: {answer}");
    return;
}

/// <summary>
/// Resolves the problem instance and type for the given day.
/// </summary>
bool TryResolveProblem(int day, [NotNullWhen(true)] out object? instance, [NotNullWhen(true)] out Type? problemType) {
    instance = null;
    problemType = null;
    
    foreach (Type type in problemTypes)
    {
        object? candidate = Activator.CreateInstance(type);
        if (candidate is null)
        {
            continue;
        }

        PropertyInfo? dayProperty = type.GetProperty("Day");
        if (dayProperty is null)
        {
            continue;
        }

        int candidateDay = (int?)dayProperty.GetValue(candidate) ?? throw new InvalidCastException("Day property is not an int.");
        if (candidateDay == day)
        {
            instance = candidate;
            problemType = type;
            break;
        }
    }

    if (instance is null || problemType is null)
    {
        Console.Error.WriteLine($"Problem for day {day} not found.");
        return false;
    }

    return true;
}

/// <summary>
/// Invokes the appropriate method on the problem instance via reflection.
/// </summary>
object? InvokeMethod(int day, int part, object? instance, Type problemType)
{
    string methodName = part == 1 ? nameof(Problem<>.SolvePartOneAsync) : nameof(Problem<>.SolvePartTwoAsync);
    MethodInfo? solver = problemType.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
    if (solver is null)
    {
        Console.Error.WriteLine($"{methodName} not found on {problemType.Name}.");
        return null;
    }

    IAsyncEnumerable<string?> lines = EmbeddedResourceReader.ReadAllLinesAsync($"Day_{day}.txt");

    object? invocationResult = solver.Invoke(instance, [lines]);
    if (invocationResult is not Task task)
    {
        Console.Error.WriteLine($"{methodName} did not return a Task.");
        return null;
    }

    return invocationResult;
}

/// <summary>
/// Gets the result from an invoked method that returns a Task.
/// </summary>
async Task<object?> GetAnswer(object? invocationResult)
{
    if (invocationResult is not Task task)
    {
        Console.Error.WriteLine("Invocation did not return a Task.");
        return null;
    }

    await task;
    task.GetAwaiter().GetResult();
    object? answer = invocationResult.GetType().GetProperty("Result")?.GetValue(invocationResult);

    return answer;
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
