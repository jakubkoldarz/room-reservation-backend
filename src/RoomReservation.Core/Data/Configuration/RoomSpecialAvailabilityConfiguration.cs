using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomReservation.Core.Entities;

namespace RoomReservation.Core.Data.Configuration
{
    public class RoomSpecialAvailabilityConfiguration : IEntityTypeConfiguration<RoomSpecialAvailability>
    {
        public void Configure(EntityTypeBuilder<RoomSpecialAvailability> roomSpecialAvailability)
        {
            roomSpecialAvailability.HasKey(rsa => rsa.Id);
            roomSpecialAvailability.HasIndex(rsa => new { rsa.RoomId, rsa.StartDate, rsa.EndDate });
        }
    }
}
