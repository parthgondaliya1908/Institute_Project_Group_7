namespace Api.Common.Extensions;

public static class StringExtensions
{
    public static string CommaSeparated(this string[] items) => string.Join(", ", items);
}
