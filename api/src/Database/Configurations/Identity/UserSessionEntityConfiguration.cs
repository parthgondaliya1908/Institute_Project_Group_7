using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Api.Database.Models.Identity;

namespace Api.Database.Configurations.Identity;

public class UserSessionEntityConfiguration : IEntityTypeConfiguration<UserSession>
{
    public void Configure(EntityTypeBuilder<UserSession> builder)
    {
        builder.ToTable("user_sessions");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityAlwaysColumn();

        builder.Property(x => x.LoggedInAt)
            .IsRequired();

        builder.Property(x => x.LoggedInWith)
            .IsRequired();

        builder.Property(x => x.SessionId)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.RefreshToken)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.RefreshTokenExpiresAt)
            .IsRequired();

        builder.Property(x => x.UserId)
            .IsRequired();

        builder
            .HasOne(x => x.User)
            .WithMany(x => x.UserSessions)
            .HasForeignKey(x => x.UserId);
    }
}
