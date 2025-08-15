namespace Api.Database.Models.Identity;

public class UserSession
{
    public long Id { get; set; }

    public DateTime LoggedInAt { get; set; }
    public Enums.LoggedInWith LoggedInWith { get; set; }
    public string SessionId { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime RefreshTokenExpiresAt { get; set; }

    public long UserId { get; set; }
    public virtual User User { get; set; } = null!;
}
