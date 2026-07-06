using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomReservation.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Data.Configuration
{
    public class VerificationCodeConfiguration : IEntityTypeConfiguration<VerificationCode>
    {
        public void Configure(EntityTypeBuilder<VerificationCode> builder)
        {
            builder.HasKey(v => v.Id);

            builder.Property(v => v.Code)
                .IsRequired()
                .HasMaxLength(6);

            builder.Property(v => v.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(40);

            builder.Property(v => v.CreatedAt)
                .IsRequired();

            builder.Property(v => v.ExpiresAt)
                .IsRequired();

            builder.Property(v => v.IsUsed)
                .IsRequired()
                .HasDefaultValue(false);

            builder.HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(v => new { v.UserId, v.Type, v.IsUsed });
        }
    }
}
