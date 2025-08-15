using CustomClaimTypes = Api.Common.Constants.Claim;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using Api.Common.States;
using Api.Database;
using Api.Database.Models.Identity;

namespace Api.Startup;

public static class DatabaseInitializer
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using IServiceScope scope = app.Services.CreateScope();
        DatabaseContext database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

        RoleManager<Role> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
        UserManager<User> userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();

        await database.Database.EnsureCreatedAsync();
        await CreateDefaultRolesAsync(database, roleManager);
        await CreateDefaultUsersAsync(database, userManager);
    }

    public static async Task CreateDefaultRolesAsync(DatabaseContext database, RoleManager<Role> roleManager)
    {
        if (await database.Roles.AnyAsync())
        {
            return;
        }

        Role[] roles = [
            new() { Name = Common.Constants.Role.Admin.Name },
            new() { Name = Common.Constants.Role.User.Name },
        ];

        foreach (Role role in roles)
        {
            await roleManager.CreateAsync(role);
        }
    }

    public static async Task CreateDefaultUsersAsync(DatabaseContext database, UserManager<User> userManager)
    {
        if (await database.Users.AnyAsync())
        {
            return;
        }

        IReadOnlyList<string> defaultAdminEmails = ["meet.thakar@glsuniversity.ac.in"];
        foreach (string defaultAdminEmail in defaultAdminEmails)
        {
            User user = new()
            {
                UserName = defaultAdminEmail,
                Email = defaultAdminEmail,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            await userManager.CreateAsync(user);
            await userManager.AddToRoleAsync(user, Common.Constants.Role.Admin.Name);

            await database.UserClaims.AddAsync(new()
            {
                ClaimType = CustomClaimTypes.Permission.Name,
                ClaimValue = Permission.All.Name,
                UserId = user.Id
            });
            await database.SaveChangesAsync();
        }
    }
}
