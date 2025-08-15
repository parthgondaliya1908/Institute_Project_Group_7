using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

using Api.Database.Configurations;
using Api.Database.Configurations.Identity;
using Api.Database.Models;
using Api.Database.Models.Identity;

namespace Api.Database;

public class DatabaseContext(DbContextOptions options) : IdentityDbContext<User, Role, long, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>(options)
{
    public DbSet<UserSession> UserSessions { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Course> Courses { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<User>().ToTable("users");
        builder.Entity<Role>().ToTable("roles");
        builder.Entity<UserRole>().ToTable("user_roles");
        builder.Entity<UserClaim>().ToTable("user_claims");
        builder.Entity<RoleClaim>().ToTable("role_claims");

        builder.Ignore<UserLogin>();
        builder.Ignore<UserToken>();

        builder.HasPostgresEnum<Enums.LoggedInWith>();

        new UserSessionEntityConfiguration().Configure(builder.Entity<UserSession>());
        new DepartmentEntityConfiguration().Configure(builder.Entity<Department>());
        new CourseEntityConfiguration().Configure(builder.Entity<Course>());
    }
}
