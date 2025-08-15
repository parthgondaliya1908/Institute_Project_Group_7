using Api.Common.Types;
using Api.Features.Auth.RefreshTokens;
using AuthApi = Api.Features.Auth.Api;

using Api.Tests.IntegrationTests.Extensions;
using Api.Tests.IntegrationTests.Utils;

namespace Api.Tests.IntegrationTests.Features.Auth.Tokens;

public class RefreshTokens : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture fixture;

    public RefreshTokens(DatabaseFixture fixture)
    {
        this.fixture = fixture;
        fixture.Initialize(GetType());
    }

    [Fact]
    public async Task InvalidAccessTokenShouldReturnUnauthorized()
    {
        try
        {
            Request request = new()
            {
                RefreshToken = "fake-refresh-token",
                SessionId = "fake-session-id"
            };

            HttpResponseMessage response = await fixture.Http.PostAsJsonAuthenticatedAsync(AuthApi.RefreshTokens.Url, request, Jwt.Invalid);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }

    [Fact]
    public async Task ExpiredAccessTokenShouldReturnOk()
    {
        try
        {
            GeneratedTokens loginTokens = await LoginWithGoogle();
            Request request = new()
            {
                RefreshToken = loginTokens.RefreshToken,
                SessionId = loginTokens.SessionId
            };

            string expiredAccessToken = TokenUtils.NukeTheAccessToken(fixture, loginTokens.AccessToken);
            HttpResponseMessage response = await fixture.Http.PostAsJsonAuthenticatedAsync(AuthApi.RefreshTokens.Url, request, expiredAccessToken);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }

    [Fact]
    public async Task ValidTokenShouldReturnOk()
    {
        try
        {
            GeneratedTokens loginTokens = await LoginWithGoogle();
            Request request = new()
            {
                RefreshToken = loginTokens.RefreshToken,
                SessionId = loginTokens.SessionId
            };

            HttpResponseMessage response = await fixture.Http.PostAsJsonAuthenticatedAsync(AuthApi.RefreshTokens.Url, request, loginTokens.AccessToken);
            GeneratedTokens? responseTokens = await response.Content.ReadFromJsonAsync<GeneratedTokens>();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            Assert.NotNull(responseTokens);
            Assert.NotNull(responseTokens.AccessToken);
            Assert.NotNull(responseTokens.RefreshToken);
            Assert.NotNull(responseTokens.SessionId);

            Assert.Equal(loginTokens.SessionId, responseTokens.SessionId);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }

    private async Task<GeneratedTokens> LoginWithGoogle()
    {
        Api.Features.Auth.Google.Request googleTokenRequest = new() { GoogleToken = GoogleTokens.CorrectToken, SessionId = null };
        HttpResponseMessage googleResponse = await fixture.Http.PostAsJsonAsync(AuthApi.Google.Url, googleTokenRequest);

        GeneratedTokens? loginTokens = await googleResponse.Content.ReadFromJsonAsync<GeneratedTokens>();
        Assert.NotNull(loginTokens);

        return loginTokens;
    }
}
