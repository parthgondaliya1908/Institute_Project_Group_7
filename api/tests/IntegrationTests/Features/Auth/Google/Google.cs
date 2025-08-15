using Api.Common.Types;
using Api.Database.Models.Identity;
using Api.Features.Auth.Google;
using AuthApi = Api.Features.Auth.Api;

namespace Api.Tests.IntegrationTests.Features.Auth.Google;

public class Google : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture fixture;

    public Google(DatabaseFixture fixture)
    {
        this.fixture = fixture;
        fixture.Initialize(GetType());
    }

    [Fact]
    public async Task UserLogsIn()
    {
        try
        {
            HttpStatusCode blankTokenStatus = await UserLogsInWithBlankToken();
            Assert.Equal(HttpStatusCode.BadRequest, blankTokenStatus);

            HttpStatusCode incorrectTokenStatus = await UserLogsInWithIncorrectToken();
            Assert.Equal(HttpStatusCode.Unauthorized, incorrectTokenStatus);

            HttpStatusCode createdStatus = await UserLogsInWithCorrectToken();
            Assert.Equal(HttpStatusCode.Created, createdStatus);

            HttpStatusCode okStatus = await UserLogsInWithCorrectToken();
            Assert.Equal(HttpStatusCode.OK, okStatus);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }

    private async Task<HttpStatusCode> UserLogsInWithBlankToken()
    {
        Request request = new()
        {
            GoogleToken = string.Empty,
            SessionId = null
        };

        HttpResponseMessage response = await fixture.Http.PostAsJsonAsync(AuthApi.Google.Url, request);
        return response.StatusCode;
    }

    private async Task<HttpStatusCode> UserLogsInWithIncorrectToken()
    {
        Request request = new()
        {
            GoogleToken = GoogleTokens.IncorrectToken,
            SessionId = null
        };

        HttpResponseMessage response = await fixture.Http.PostAsJsonAsync(AuthApi.Google.Url, request);
        return response.StatusCode;
    }

    private async Task<HttpStatusCode> UserLogsInWithCorrectToken()
    {
        Request request = new()
        {
            GoogleToken = GoogleTokens.CorrectToken,
            SessionId = null
        };

        HttpResponseMessage response = await fixture.Http.PostAsJsonAsync(AuthApi.Google.Url, request);

        GeneratedTokens? payload = await response.Content.ReadFromJsonAsync<GeneratedTokens>();
        Assert.NotNull(payload);
        Assert.NotNull(payload.AccessToken);
        Assert.NotNull(payload.RefreshToken);
        Assert.NotNull(payload.SessionId);

        User? user = await fixture.UserManager.FindByEmailAsync(MockGoogleUser.Email);
        Assert.NotNull(user);

        bool hasUserRole = await fixture.UserManager.IsInRoleAsync(user, Common.Constants.Role.User.Name);
        Assert.True(hasUserRole);

        UserSession? userSession = await fixture.Database.UserSessions
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.UserId == user.Id && u.RefreshToken == payload.RefreshToken && u.SessionId == payload.SessionId);

        Assert.NotNull(userSession);

        return response.StatusCode;
    }
}
