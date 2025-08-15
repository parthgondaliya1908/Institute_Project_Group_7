using Api.Common.Types;
using Api.Database.Models.Identity;
using Api.Features.Auth.Google;
using Api.Tests.IntegrationTests.Extensions;
using AuthApi = Api.Features.Auth.Api;

namespace Api.Tests.IntegrationTests.Features.Auth.Google;

public class GoogleWeb : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture fixture;

    public GoogleWeb(DatabaseFixture fixture)
    {
        this.fixture = fixture;
        fixture.Initialize(GetType());
    }
    
    [Fact]
    public async Task UserLogsIn()
    {
        try
        {
            HttpStatusCode blankTokenStatus = await UserLogsWithBlankToken();
            Assert.Equal(HttpStatusCode.BadRequest, blankTokenStatus);

            HttpStatusCode incorrectTokenStatus = await UserLogsWithIncorrectToken();
            Assert.Equal(HttpStatusCode.Unauthorized, incorrectTokenStatus);

            HttpStatusCode createdStatus = await UserLogsWithCorrectToken();
            Assert.Equal(HttpStatusCode.Created, createdStatus);

            HttpStatusCode okStatus = await UserLogsWithCorrectToken();
            Assert.Equal(HttpStatusCode.OK, okStatus);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }

    private async Task<HttpStatusCode> UserLogsWithBlankToken()
    {
        Request request = new()
        {
            GoogleToken = string.Empty,
            SessionId = null
        };

        HttpResponseMessage response = await fixture.Http.PostAsJsonAsync(AuthApi.Google.Url, request);
        return response.StatusCode;
    }

    private async Task<HttpStatusCode> UserLogsWithIncorrectToken()
    {
        Request request = new()
        {
            GoogleToken = GoogleTokens.IncorrectToken,
            SessionId = null
        };

        HttpResponseMessage response = await fixture.Http.PostAsJsonAsync(AuthApi.GoogleWeb.Url, request);
        return response.StatusCode;
    }

    private async Task<HttpStatusCode> UserLogsWithCorrectToken()
    {
        Request request = new()
        {
            GoogleToken = GoogleTokens.CorrectToken,
            SessionId = null
        };

        HttpResponseMessage response = await fixture.Http.PostAsJsonAsync(AuthApi.GoogleWeb.Url, request);
        OnlyAccessToken? payload = await response.Content.ReadFromJsonAsync<OnlyAccessToken>();

        Assert.NotNull(payload);
        Assert.NotNull(payload.AccessToken);

        string? refreshToken = response.GetCookieValue(TokenType.RefreshToken.Name);
        string? sessionId = response.GetCookieValue(TokenType.SessionId.Name);

        Assert.NotNull(refreshToken);
        Assert.NotNull(sessionId);

        User? user = await fixture.UserManager.FindByEmailAsync(MockGoogleUser.Email);
        Assert.NotNull(user);

        bool hasUserRole = await fixture.UserManager.IsInRoleAsync(user, Common.Constants.Role.User.Name);
        Assert.True(hasUserRole);

        UserSession? userSession = await fixture.Database.UserSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == user.Id && u.RefreshToken == refreshToken && u.SessionId == sessionId);

        Assert.NotNull(userSession);

        return response.StatusCode;
    }
}
