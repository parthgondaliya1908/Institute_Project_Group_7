using System.Net.Http.Headers;

namespace Api.Tests.IntegrationTests.Extensions;

public static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> GetAuthenticatedAsync(this HttpClient client, string url, string accessToken)
    {
        return await SendAuthenticatedAsync(client, HttpMethod.Get, url, accessToken);
    }

    public static async Task<HttpResponseMessage> PostAsJsonAuthenticatedAsync<T>(this HttpClient client, string url, T value, string accessToken)
    {
        return await client.SendAsJsonAuthenticatedAsync(HttpMethod.Post, url, value, accessToken);
    }

    public static async Task<HttpResponseMessage> PutAsJsonAuthenticatedAsync<T>(this HttpClient client, string url, T value, string accessToken)
    {
        return await client.SendAsJsonAuthenticatedAsync(HttpMethod.Put, url, value, accessToken);
    }

    public static async Task<HttpResponseMessage> PatchAsJsonAuthenticatedAsync<T>(this HttpClient client, string url, T value, string accessToken)
    {
        return await client.SendAsJsonAuthenticatedAsync(HttpMethod.Patch, url, value, accessToken);
    }

    public static async Task<HttpResponseMessage> DeleteAuthenticatedAsync(this HttpClient client, string url, string accessToken)
    {
        return await SendAuthenticatedAsync(client, HttpMethod.Delete, url, accessToken);
    }

    public static async Task<HttpResponseMessage> SendAuthenticatedAsync(this HttpClient client, HttpMethod method, string url, string accessToken)
    {
        HttpRequestMessage request = new(method, url);

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        HttpResponseMessage response = await client.SendAsync(request);

        return response;
    }

    public static async Task<HttpResponseMessage> SendAsJsonAuthenticatedAsync<T>(this HttpClient client, HttpMethod method, string url, T value, string accessToken)
    {
        HttpRequestMessage request = new(method, url)
        {
            Content = JsonContent.Create(value)
        };

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        HttpResponseMessage response = await client.SendAsync(request);

        return response;
    }
}
