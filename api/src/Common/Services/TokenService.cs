using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using BuiltInClaim = System.Security.Claims.Claim;
using BuiltInClaimTypes = System.Security.Claims.ClaimTypes;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Api.Common.Constants;
using Api.Common.Options;
using CustomClaimTypes = Api.Common.Constants.Claim;

namespace Api.Common.Services;

public interface ITokenService
{
    string GenerateAccessToken(long userId, string email, string[] roles, BuiltInClaim[] permissions);
    string GenerateRefreshToken();
    string GenerateSessionId();
}

public class TokenService(IOptions<JwtOptions> jwtOptions) : ITokenService
{
    public string GenerateAccessToken(long userId, string email, string[] roles, BuiltInClaim[] permissions)
    {
        DateTime issuedAt = DateTime.UtcNow;
        DateTime expiresAt = DateTime.UtcNow.AddMinutes(jwtOptions.Value.AccessTokenExpiryInMinutes);

        ClaimsIdentity claims = new();
        claims.AddClaims([
            new(BuiltInClaimTypes.Email, email.ToString()),
            new(CustomClaimTypes.UserId.Name, userId.ToString()),
            new(CustomClaimTypes.TokenType.Name, TokenType.AccessToken.Name),
        ]);
        claims.AddClaims(roles.Select(role => new BuiltInClaim(BuiltInClaimTypes.Role, role)));
        claims.AddClaims(permissions);

        SymmetricSecurityKey securityKey = new(Encoding.UTF8.GetBytes(jwtOptions.Value.SecurityKey));
        SigningCredentials credentials = new(securityKey, SecurityAlgorithms.HmacSha256Signature);
        SecurityTokenDescriptor tokenDescriptor = new()
        {
            SigningCredentials = credentials,
            Expires = expiresAt,
            Issuer = jwtOptions.Value.Issuer,
            IssuedAt = issuedAt,
            Subject = claims,
        };

        JwtSecurityTokenHandler tokenHandler = new();
        SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);

        string token = tokenHandler.WriteToken(securityToken);
        return token;
    }

    public string GenerateRefreshToken()
    {
        return $"{Guid.NewGuid()}-{Guid.NewGuid()}-{Guid.NewGuid()}";
    }

    public string GenerateSessionId()
    {
        return Guid.NewGuid().ToString();
    }
}
