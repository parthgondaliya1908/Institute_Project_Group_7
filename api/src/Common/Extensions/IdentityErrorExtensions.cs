using Microsoft.AspNetCore.Identity;

namespace Api.Common.Extensions;

public static class IdentityErrorExtensions
{
    public static string CommaSeparated(this IEnumerable<IdentityError> items)
    {
        char separator = ',';
        string message = items.Aggregate("", (msg, result) => string.Join(separator, result.Description));

        return message.Trim(separator);
    }
}
