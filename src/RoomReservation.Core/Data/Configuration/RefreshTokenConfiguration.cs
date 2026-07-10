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
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.HasKey(rt => rt.Id);

            builder.HasIndex(rt => rt.ExpiresAt);
            builder.HasIndex(rt => rt.UserId);
            builder.HasIndex(rt => rt.TokenHash).IsUnique();

            builder.Property(rt => rt.TokenHash).IsRequired().HasMaxLength(100);
            builder.Property(rt => rt.IpAddress).HasMaxLength(30);
            builder.Property(rt => rt.UserAgent).HasMaxLength(500);

            builder.Property(rt => rt.CreatedAt).IsRequired();
            builder.Property(rt => rt.ExpiresAt).IsRequired();
        }
    }
}
