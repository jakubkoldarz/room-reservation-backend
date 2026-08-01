using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomReservation.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

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

            token.Property(rt => rt.TokenHash).IsRequired().HasMaxLength(100);
            token.Property(rt => rt.IpAddress).HasMaxLength(30);
            token.Property(rt => rt.UserAgent).HasMaxLength(500);

            token.Property(rt => rt.CreatedAt).IsRequired();
            token.Property(rt => rt.ExpiresAt).IsRequired();
        }
    }
}
