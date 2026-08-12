using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;

namespace RoomReservation.Core.Data.Configuration
{
    public class ReservationConfiguration : IEntityTypeConfiguration<Reservation>
    {
        public void Configure(EntityTypeBuilder<Reservation> reservation)
        {
            reservation.HasKey(r => r.Id);
            reservation.Property(r => r.Purpose).HasMaxLength(100);
            reservation.Property(r => r.Status).HasConversion<string>().HasDefaultValue(ReservationStatus.Pending);

            reservation.HasOne(r => r.CreatedBy)
                .WithMany(u => u.Reservations)
                .HasForeignKey(r => r.CreatedById)
                .OnDelete(DeleteBehavior.SetNull);

            reservation.HasOne(r => r.ApprovedBy)
                .WithMany()
                .HasForeignKey(r => r.ApprovedById)
                .OnDelete(DeleteBehavior.SetNull);

            reservation.HasOne(r => r.Room)
                .WithMany(rm => rm.Reservations)
                .HasForeignKey(r => r.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
