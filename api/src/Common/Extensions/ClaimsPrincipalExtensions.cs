using System.Security.Claims;

using CustomClaim = Api.Common.Constants.Claim;

namespace Api.Common.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static long GetUserId(this ClaimsPrincipal claims)
    {
        string id = claims.FindFirstValue(CustomClaim.UserId.Name)!;
        return long.Parse(id);
    }
}
