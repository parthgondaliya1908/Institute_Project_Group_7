namespace Api.Common.Options;

public class CookieOptions
{
    public const string SectionName = "Cookies";

    public string SameSite { get; set; } = null!;
    public bool Secure { get; set; }
    public bool HttpOnly { get; set; }
}
