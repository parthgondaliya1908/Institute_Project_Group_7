namespace Api.Common.Types;

public record GeneratedTokens(string AccessToken, string RefreshToken, string SessionId);
public record OnlyAccessToken(string AccessToken);
