namespace Api.Common.Constants;

public readonly record struct TokenType(string Name)
{
    public static readonly TokenType AccessToken = new("AccessToken");
    public static readonly TokenType RefreshToken = new("RefreshToken");
    public static readonly TokenType SessionId = new("SessionId");
}

public readonly record struct Scheme(string Name)
{
    public static readonly Scheme ValidJwt = new("ValidJwtScheme");
    public static readonly Scheme ExpiredJwt = new("ExpiredJwtScheme");
}

public readonly record struct Role(string Name)
{
    public static readonly Role Admin = new("Admin");
    public static readonly Role User = new("User");
}

public readonly record struct Policy(string Name)
{
    public static readonly Policy RequireValidJwt = new("RequireValidJwt");
    public static readonly Policy RequireValidJwtAdmin = new("RequireValidJwtAdmin");
    public static readonly Policy RequireValidJwtUser = new("RequireValidJwtUser");
    
    public static readonly Policy AllowExpiredJwt = new("AllowExpiredJwt");
    public static readonly Policy AllowExpiredJwtAdmin = new("AllowExpiredJwtAdmin");
    public static readonly Policy AllowExpiredJwtUser = new("AllowExpiredJwtUser");
}

public readonly record struct Claim(string Name)
{
    public static readonly Claim UserId = new("UserId");
    public static readonly Claim TokenType = new("TokenType");
    public static readonly Claim Permission = new("Permission");
}
