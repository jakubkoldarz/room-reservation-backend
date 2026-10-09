using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomReservation.Core.Entities;

namespace RoomReservation.Core.Data.Configuration
{
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> token)
        {
            token.HasKey(rt => rt.Id);

            token.HasIndex(rt => rt.ExpiresAt);
            token.HasIndex(rt => rt.UserId);
            token.HasIndex(rt => rt.TokenHash).IsUnique();

            token.Property(rt => rt.TokenHash).HasMaxLength(100);
            token.Property(rt => rt.IpAddress).HasMaxLength(30);
            token.Property(rt => rt.UserAgent).HasMaxLength(500);
        }
    }
}
