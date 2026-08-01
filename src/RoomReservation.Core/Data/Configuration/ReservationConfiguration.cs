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
            reservation.Property(r => r.CreatedById).IsRequired();
            reservation.Property(r => r.RoomId).IsRequired();
            reservation.Property(r => r.StartTime).IsRequired();
            reservation.Property(r => r.EndTime).IsRequired();
            reservation.Property(r => r.Purpose).HasMaxLength(100);
            reservation.Property(r => r.Status).IsRequired().HasConversion<string>().HasDefaultValue(ReservationStatus.Pending);
            reservation.Property(r => r.CreatedAt).IsRequired();

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
