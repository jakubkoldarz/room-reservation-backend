using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomReservation.Core.Entities;

namespace RoomReservation.Core.Data.Configuration
{
    public class BuildingConfiguration : IEntityTypeConfiguration<Building>
    {
        public void Configure(EntityTypeBuilder<Building> building)
        {
            building.HasKey(b => b.Id);

            building.Property(b => b.Name).IsRequired().HasMaxLength(100);
            building.HasIndex(b => b.Name).IsUnique();
            building.Property(b => b.Identifier).HasMaxLength(20);
            building.Property(b => b.Street).HasMaxLength(50);
            building.Property(b => b.City).HasMaxLength(50);
            building.Property(b => b.PostalCode).HasMaxLength(6);

            building.HasMany(b => b.Availabilities)
                .WithOne(a => a.Building)
                .HasForeignKey(a => a.BuildingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
