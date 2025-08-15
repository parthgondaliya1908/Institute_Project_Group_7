using Microsoft.Extensions.Options;

using Google.Apis.Auth;

using Api.Common.Options;
using Api.Common.Types;

namespace Api.Common.Services;

public interface IGoogleTokenService
{
    Task<Result<GoogleUser>> VerifyGoogleTokenAsync(string googleToken);
}

public class GoogleTokenService(IOptions<OAuthOptions> oauthOptions) : IGoogleTokenService
{
    public async Task<Result<GoogleUser>> VerifyGoogleTokenAsync(string googleToken)
    {
        try
        {
            GoogleJsonWebSignature.ValidationSettings settings = new() { Audience = [oauthOptions.Value.GoogleClientId] };
            GoogleJsonWebSignature.Payload payload = await GoogleJsonWebSignature.ValidateAsync(googleToken, settings);

            if (payload.Email is null)
            {
                return Result.Fail("Failed to verify Google token. Reason: Email is invalid or doesn't exist.");
            }

            GoogleUser googleUser = new(payload.Subject, payload.Email, payload.Name, payload.Picture);
            return Result.Success(googleUser);
        }
        catch (InvalidJwtException ex)
        {
            return Result.Fail($"Failed to verify Google token. Reason: {ex.Message}");
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to verify Google token. Reason: {ex.Message}");
        }
    }
}
