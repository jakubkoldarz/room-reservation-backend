using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomReservation.Core.Entities;

namespace RoomReservation.Core.Data.Configuration
{
    public class BuildingAvailabilityConfiguration : IEntityTypeConfiguration<BuildingAvailability>
    {
        public void Configure(EntityTypeBuilder<BuildingAvailability> buildingAvailability)
        {
            buildingAvailability.HasKey(ba => ba.Id);
        }
    }
}
