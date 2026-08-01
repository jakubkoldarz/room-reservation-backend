using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomReservation.Core.Entities;

namespace RoomReservation.Core.Data.Configuration
{
    public class EquipmentConfiguration : IEntityTypeConfiguration<Equipment>
    {
        public void Configure(EntityTypeBuilder<Equipment> equipment)
        {
            equipment.HasKey(e => e.Id);
            equipment.HasIndex(e => e.Name).IsUnique();
            equipment.Property(e => e.Name).IsRequired().HasMaxLength(50);
            equipment.Property(e => e.Icon).IsRequired().HasMaxLength(50);
        }
    }
}
