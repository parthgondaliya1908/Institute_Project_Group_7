using System.Security.Claims;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

using Api.Common.Extensions;
using Api.Common.Options;
using Api.Common.Services;
using Api.Common.Types;
using Api.Database.Models.Identity;
using Api.Database.Models;
using Api.Common.Repositories;

namespace Api.Features.Auth.Google;

public class Service(
    [FromServices] IUserSessionRepository userSessionRepo,
    [FromServices] ITokenService tokenService,
    [FromServices] IGoogleTokenService googleTokenService,
    [FromServices] IOptions<JwtOptions> jwtOptions,
    [FromServices] UserManager<User> userManager
)
{
    public async Task<Result<GeneratedTokens>> LoginWithGoogleAsync(string googleToken, string? sessionId, CancellationToken cancellationToken)
    {
        Result<GoogleUser> googleUserResult = await googleTokenService.VerifyGoogleTokenAsync(googleToken);
        if (!googleUserResult.IsSuccess)
        {
            return Result.UnauthorizedError();
        }

        GoogleUser googleUser = googleUserResult.Value;
        User? user = await userManager.FindByEmailAsync(googleUser.Email);

        IList<string> userRoles;
        IList<Claim> permissions;

        bool newUser = false;
        if (user is null)
        {
            user = new User() { Email = googleUser.Email, UserName = googleUser.Email };

            IdentityResult createUserResult = await userManager.CreateAsync(user);
            if (!createUserResult.Succeeded)
            {
                return Result.CannotProcessError($"Failed to create user. Reason: {createUserResult.Errors.CommaSeparated()}");
            }

            IdentityResult addToRoleResult = await userManager.AddToRoleAsync(user, Common.Constants.Role.User.Name);
            if (!addToRoleResult.Succeeded)
            {
                return Result.CannotProcessError($"Failed to assign role to user. Reason: {addToRoleResult.Errors.CommaSeparated()}");
            }

            userRoles = [Common.Constants.Role.User.Name];
            permissions = [];

            newUser = true;
        }
        else
        {
            userRoles = await userManager.GetRolesAsync(user);
            permissions = await userManager.GetClaimsAsync(user);
        }

        string accessToken = tokenService.GenerateAccessToken(user.Id, user.Email!, [.. userRoles], [.. permissions]);
        string refreshToken = tokenService.GenerateRefreshToken();

        DateTime loggedInAt = DateTime.UtcNow;
        DateTime refreshTokenExpiry = loggedInAt.AddDays(jwtOptions.Value.RefreshTokenExpiryInDays);

        UserSession? userSession = sessionId is not null ? await userSessionRepo.GetUserSessionAsync(user.Id, sessionId, cancellationToken) : null;
        if (userSession is null)
        {
            string newSessionId = tokenService.GenerateSessionId();
            userSession = new()
            {
                UserId = user.Id,
                SessionId = newSessionId,
                LoggedInWith = Enums.LoggedInWith.Google,
                LoggedInAt = loggedInAt,
            };
        }

        userSession.LoggedInAt = loggedInAt;
        userSession.RefreshToken = refreshToken;
        userSession.RefreshTokenExpiresAt = refreshTokenExpiry;

        await userSessionRepo.AddOrUpdateUserSessionAsync(userSession, cancellationToken);
        GeneratedTokens generatedTokens = new(accessToken, refreshToken, userSession.SessionId);

        return newUser ? Result.Created(generatedTokens) : Result.Success(generatedTokens);
    }
}
