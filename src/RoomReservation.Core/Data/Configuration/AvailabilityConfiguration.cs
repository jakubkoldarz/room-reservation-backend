using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomReservation.Core.Entities;

namespace RoomReservation.Core.Data.Configuration
{
    public class AvailabilityConfiguration : IEntityTypeConfiguration<Availability>
    {
        public void Configure(EntityTypeBuilder<Availability> availability)
        {
            availability.HasKey(a => a.Id);

            availability.HasOne(a => a.Room)
                .WithMany(r => r.Availabilities)
                .HasForeignKey(a => a.RoomId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            availability.HasOne(a => a.Building)
                .WithMany(b => b.Availabilities)
                .HasForeignKey(a => a.BuildingId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Cascade);

            availability.HasIndex(a => new { a.RoomId, a.DayOfWeek });
            availability.HasIndex(a => new { a.BuildingId, a.DayOfWeek });
        }
    }
}
