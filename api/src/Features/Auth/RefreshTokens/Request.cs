namespace Api.Features.Auth.RefreshTokens;

public class Request
{
    public string RefreshToken { get; set; } = null!;
    public string SessionId { get; set; } = null!;
}
