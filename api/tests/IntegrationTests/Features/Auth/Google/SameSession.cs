using Api.Common.Types;
using Api.Features.Auth.Google;
using AuthApi = Api.Features.Auth.Api;

namespace Api.Tests.IntegrationTests.Features.Auth.Google;

public class SameSession : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture fixture;

    public SameSession(DatabaseFixture fixture)
    {
        this.fixture = fixture;
        fixture.Initialize(GetType());
    }

    [Fact]
    public async Task WhenUserLogsInFromOneClientMultipleTimesMultipleSessionsShouldNotBeCreated()
    {
        try
        {
            string sessionId1 = await UserLogsWithCorrectToken();
            string sessionId2 = await UserLogsWithCorrectToken(sessionId1);

            Assert.Equal(sessionId1, sessionId2);

            int sessionCount = await fixture.Database.UserSessions
                .Where(us => us.SessionId == sessionId1)
                .CountAsync();

            Assert.Equal(1, sessionCount);
        }
        catch
        {
            fixture.MarkTestsFailed();
            throw;
        }
    }

    private async Task<string> UserLogsWithCorrectToken(string? sessionId = null)
    {
        Request request = new()
        {
            GoogleToken = GoogleTokens.CorrectToken,
            SessionId = sessionId
        };

        HttpResponseMessage response = await fixture.Http.PostAsJsonAsync(AuthApi.Google.Url, request);
        GeneratedTokens? payload = await response.Content.ReadFromJsonAsync<GeneratedTokens>();

        Assert.NotNull(payload);
        Assert.NotNull(payload.SessionId);

        return payload.SessionId;
    }
}
