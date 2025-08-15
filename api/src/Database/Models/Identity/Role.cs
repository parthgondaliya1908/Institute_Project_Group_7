using Microsoft.AspNetCore.Identity;

namespace Api.Database.Models.Identity;

public class Role : IdentityRole<long>
{
    public Role() : base() { }
    public Role(string name) : base(name) { }
}
