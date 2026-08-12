using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomReservation.Core.Entities;

namespace RoomReservation.Core.Data.Configuration
{
    public class RoomAvailabilityConfiguration : IEntityTypeConfiguration<RoomAvailability>
    {
        public void Configure(EntityTypeBuilder<RoomAvailability> roomAvailability)
        {
            roomAvailability.HasKey(ra => ra.Id);
        }
    }
}
