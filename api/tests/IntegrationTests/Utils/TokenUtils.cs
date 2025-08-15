using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Api.Common.Options;
using Api.Common.States;
using CustomClaimTypes = Api.Common.Constants.Claim;

namespace Api.Tests.IntegrationTests.Utils;

public static class TokenUtils
{
    public static string GenerateAccessToken(JwtOptions jwtOptions, long userId, Permission[] permissions)
    {
        DateTime issuedAt = DateTime.UtcNow;
        DateTime expiresAt = DateTime.UtcNow.AddYears(1);

        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(jwtOptions.SecurityKey));
        SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256Signature);

        List<System.Security.Claims.Claim> claims = [new(CustomClaimTypes.UserId.Name, userId.ToString())];
        foreach (Permission permission in permissions)
        {
            claims.Add(new(CustomClaimTypes.Permission.Name, permission.Name));
        }

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            SigningCredentials = credentials,
            Expires = expiresAt,
            Issuer = jwtOptions.Issuer,
            IssuedAt = issuedAt,
            NotBefore = issuedAt,
            Subject = new ClaimsIdentity(claims),
        };

        JwtSecurityTokenHandler tokenHandler = new();
        SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);

        string token = tokenHandler.WriteToken(securityToken);
        return token;
    }

    public static string NukeTheAccessToken(DatabaseFixture fixture, string accessToken)
    {
        long userId = GetUserIdFromToken(accessToken);
        IOptions<JwtOptions> jwtOptions = fixture.Factory.Services.GetRequiredService<IOptions<JwtOptions>>();

        DateTime issuedAt = DateTime.UtcNow.AddDays(-2);
        DateTime expiresAt = DateTime.UtcNow.AddDays(-1);

        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(jwtOptions.Value.SecurityKey));
        SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256Signature);
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            SigningCredentials = credentials,
            Expires = expiresAt,
            Issuer = jwtOptions.Value.Issuer,
            IssuedAt = issuedAt,
            NotBefore = issuedAt,
            Subject = new ClaimsIdentity([new(CustomClaimTypes.UserId.Name, userId.ToString())]),
        };

        JwtSecurityTokenHandler tokenHandler = new();
        SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);

        string token = tokenHandler.WriteToken(securityToken);
        return token;
    }

    private static long GetUserIdFromToken(string accessToken)
    {
        JwtSecurityTokenHandler handler = new();
        JwtSecurityToken jwtToken = handler.ReadJwtToken(accessToken);

        return long.Parse(jwtToken.Claims.First(c => c.Type == CustomClaimTypes.UserId.Name).Value);
    }
}
