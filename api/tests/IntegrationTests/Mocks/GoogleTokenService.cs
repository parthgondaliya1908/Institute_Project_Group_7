using Api.Common.Services;
using Api.Common.Types;

namespace Api.Tests.IntegrationTests.Mocks;

public class GoogleTokenService : IGoogleTokenService
{
    private GoogleUser? mockGoogleUser = null;

    public void SetMockGoogleUser(GoogleUser googleUser) => mockGoogleUser = googleUser;

    public Task<Result<GoogleUser>> VerifyGoogleTokenAsync(string googleToken)
    {
        if (mockGoogleUser is null)
        {
            return Task.FromResult<Result<GoogleUser>>(Result.Fail("Mock Google user is not set."));
        }

        Result<GoogleUser> result = googleToken == GoogleTokens.IncorrectToken
            ? Result.UnauthorizedError()
            : Result.Success(mockGoogleUser);
        
        return Task.FromResult(result);
    }
}
