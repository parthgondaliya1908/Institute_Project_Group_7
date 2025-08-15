namespace Api.Features.Auth;

public record class Api(string Url, string Description)
{
    public const string Tag = "Auth";
    public const string BaseUrl = $"{ApiBase.Path}/auth";

    public static readonly Api Google = new(
        Url: $"{BaseUrl}/google",
        Description: "Verfies the Google token and returns access token, refresh token, and session id of the user as JSON response"
    );

    public static readonly Api GoogleWeb = new(
        Url: $"{BaseUrl}/google/web",
        Description: "Verfies the Google token and returns access token as JSON; refresh token, and session id as cookies"
    );

    public static readonly Api RefreshTokens = new(
        Url: $"{BaseUrl}/refresh-tokens",
        Description: "Verfies the refresh token and returns access token, refresh token, and session id of the user as JSON response"
    );

    public static readonly Api RefreshTokensWeb = new(
        Url: $"{BaseUrl}/refresh-tokens/web",
        Description: "Verfies the refresh token and returns access token as JSON; refresh token, and session id as cookies"
    );

    public static readonly Api Logout = new(
        Url: $"{BaseUrl}/logout",
        Description: "Logs out the user"
    );

    public static readonly Api LogoutWeb = new(
        Url: $"{BaseUrl}/logout/web",
        Description: "Logs out the user"
    );

    public static readonly Api LogoutAll = new(
        Url: $"{BaseUrl}/logout-all",
        Description: "Logs out all sessions of the user"
    );

    public static readonly Api LogoutAllWeb = new(
        Url: $"{BaseUrl}/logout-all/web",
        Description: "Logs out all sessions of the user"
    );
}
