using System.Security.Claims;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Api.Common.Options;
using Api.Common.Repositories;
using Api.Common.Services;
using Api.Common.Types;
using Api.Database.Models.Identity;

namespace Api.Features.Auth.RefreshTokens;

public class Service(
    [FromServices] IUserSessionRepository userSessionRepo,
    [FromServices] ITokenService tokenService,
    [FromServices] IOptions<JwtOptions> jwtOptions,
    [FromServices] UserManager<User> userManager
)
{
    public async Task<Result<GeneratedTokens>> GenerateNewTokensAsync(long userId, string refreshToken, string sessionId, CancellationToken cancellationToken)
    {
        UserSession? userSession = await userSessionRepo.GetUserSessionAsync(userId, sessionId, cancellationToken);
        if (userSession is null || userSession.RefreshToken != refreshToken || userSession.RefreshTokenExpiresAt < DateTime.UtcNow)
        {
            return Result.UnauthorizedError();
        }

        User? user = await userManager.FindByIdAsync(userId.ToString());
        if (user is null)
        {
            return Result.UnauthorizedError();
        }

        IList<string> userRoles = await userManager.GetRolesAsync(user);
        IList<Claim> permissions = await userManager.GetClaimsAsync(user);

        string newAccessToken = tokenService.GenerateAccessToken(user.Id, user.Email!, [.. userRoles], [.. permissions]);
        string newRefreshToken = tokenService.GenerateRefreshToken();
        DateTime newRefreshTokenExpiry = DateTime.UtcNow.AddDays(jwtOptions.Value.RefreshTokenExpiryInDays);

        userSession.RefreshToken = newRefreshToken;
        userSession.RefreshTokenExpiresAt = newRefreshTokenExpiry;

        await userSessionRepo.AddOrUpdateUserSessionAsync(userSession, cancellationToken);
        return Result.Success(new GeneratedTokens(newAccessToken, newRefreshToken, sessionId));
    } 
}
