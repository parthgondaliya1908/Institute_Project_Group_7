using Microsoft.AspNetCore.Identity;

namespace Api.Database.Models.Identity;

public class User : IdentityUser<long>
{
    public virtual List<UserSession> UserSessions { get; set; } = null!;
}
