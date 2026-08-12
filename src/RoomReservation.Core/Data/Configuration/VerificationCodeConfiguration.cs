using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomReservation.Core.Entities;

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

            code.Property(v => v.Type).HasConversion<string>().HasMaxLength(40);
            code.Property(v => v.IsUsed).HasDefaultValue(false);

            code.HasOne(v => v.User)
                .WithMany()
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            code.HasIndex(v => new { v.UserId, v.Type, v.IsUsed });
        }
    }
}
