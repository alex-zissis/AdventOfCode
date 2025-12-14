namespace AdventOfCode;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class DayAttribute(int day) : Attribute
{
    public int Day { get; } = day;
}
