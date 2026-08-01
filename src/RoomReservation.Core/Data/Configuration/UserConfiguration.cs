using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomReservation.Core.Entities;

namespace RoomReservation.Core.Data.Configuration
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> user)
        {
            user.HasKey(u => u.Id);
            user.HasIndex(x => x.Email).IsUnique();
            user.Property(x => x.Email).IsRequired();
            user.Property(u => u.Firstname).HasMaxLength(50);
            user.Property(u => u.Lastname).HasMaxLength(100);

            user.HasOne(u => u.Role).WithMany().HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            user.HasMany(u => u.RefreshTokens)
                .WithOne(rt => rt.User)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
