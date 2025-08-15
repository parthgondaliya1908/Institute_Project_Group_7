namespace Api.Common.Options;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = null!;
    public string SecurityKey { get; init; } = null!;
    public int AccessTokenExpiryInMinutes { get; init; }
    public int RefreshTokenExpiryInDays { get; init; }
    public bool AllowInsecureHttp { get; init; }
}
