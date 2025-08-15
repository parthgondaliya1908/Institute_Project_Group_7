using System.Text.Json;

namespace Api.Common.Options;

public static class AppJsonSerializerOptions
{
    private static JsonSerializerOptions options = null!;

    public static JsonSerializerOptions Instance
    {
        get 
        {
            if (options is not null)
                return options;

            options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            return options;
        }
    }
}
