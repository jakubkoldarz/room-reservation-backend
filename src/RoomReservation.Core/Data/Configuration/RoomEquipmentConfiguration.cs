using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomReservation.Core.Entities;

namespace RoomReservation.Core.Data.Configuration
{
    public class RoomEquipmentConfiguration : IEntityTypeConfiguration<RoomEquipment>
    {
        public void Configure(EntityTypeBuilder<RoomEquipment> roomEquipment)
        {
            roomEquipment.HasKey(re => new { re.RoomId, re.EquipmentId });
            roomEquipment.Property(re => re.RoomId).IsRequired();
            roomEquipment.Property(re => re.EquipmentId).IsRequired();

            roomEquipment.HasOne(re => re.Room)
                .WithMany(r => r.RoomEquipments)
                .HasForeignKey(re => re.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            roomEquipment.HasOne(re => re.Equipment)
                .WithMany()
                .HasForeignKey(re => re.EquipmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }   
    }
}
