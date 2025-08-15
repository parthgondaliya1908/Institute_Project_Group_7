namespace Api.Tests.IntegrationTests.Extensions;

public static class HttpResponseMessageExtensions
{
    public static string? GetCookieValue(this HttpResponseMessage response, string key)
    {
        IEnumerable<string> cookies = response.Headers.GetValues("Set-Cookie");
        string? foundCookie = cookies.FirstOrDefault(c => c.Contains(key));

        if (foundCookie is null)
            return null;

        return foundCookie.Split('=')[1].Split(';')[0];
    }
}
