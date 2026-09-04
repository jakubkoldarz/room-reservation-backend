using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomReservation.Core.Entities;

namespace RoomReservation.Core.Data.Configuration
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> room)
        {
            room.HasKey(r => r.Id);
            room.HasIndex(r => new { r.BuildingId, r.Identifier }).IsUnique();
            room.Property(r => r.Identifier).HasMaxLength(50);

            room.HasOne(r => r.Building)
                .WithMany(b => b.Rooms)
                .HasForeignKey(r => r.BuildingId)
                .OnDelete(DeleteBehavior.Restrict);

            room.HasMany(r => r.Availabilities)
                .WithOne(ra => ra.Room)
                .HasForeignKey(ra => ra.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
