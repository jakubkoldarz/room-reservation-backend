using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomReservation.Core.Entities;

namespace RoomReservation.Core.Data.Configuration
{
    public class BuildingSpecialAvailabilityConfiguration : IEntityTypeConfiguration<BuildingSpecialAvailability>
    {
        public void Configure(EntityTypeBuilder<BuildingSpecialAvailability> buildingSpecialAvailability)
        {
            buildingSpecialAvailability.HasKey(ba => ba.Id);
            buildingSpecialAvailability.HasIndex(ba => new { ba.BuildingId, ba.StartDate, ba.EndDate });
        }
    }
}
