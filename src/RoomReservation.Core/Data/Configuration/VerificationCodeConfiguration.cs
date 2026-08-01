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
        public void Configure(EntityTypeBuilder<VerificationCode> code)
        {
            code.HasKey(v => v.Id);

            code.Property(v => v.Code)
                            .IsRequired()
                            .HasMaxLength(6);

            code.Property(v => v.Type)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(40);

            code.Property(v => v.CreatedAt)
                .IsRequired();

            code.Property(v => v.ExpiresAt)
                .IsRequired();

            code.Property(v => v.IsUsed)
                .IsRequired()
                .HasDefaultValue(false);

            code.HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            code.HasIndex(v => new { v.UserId, v.Type, v.IsUsed });
        }
    }
}
