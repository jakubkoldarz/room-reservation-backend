using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RoomReservation.Core.Entities;

namespace RoomReservation.Core.Data.Configuration
{
    public class EventConfiguration : IEntityTypeConfiguration<Event>
    {
        public void Configure(EntityTypeBuilder<Event> builder)
        {
            builder.HasKey(e => e.Id);

            builder.HasMany(e => e.Rooms)
                .WithMany(r => r.Events) 
                .UsingEntity(j => j.ToTable("EventRooms"));
        }
    }
}
