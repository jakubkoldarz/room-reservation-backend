using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Entities
{
    public class RoomAvailability
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public Guid RoomId { get; set; }
        public Room Room { get; set; } = null!;

        public required TimeOnly StartTime { get; set; }
        public required TimeOnly EndTime { get; set; }
        public required DayOfWeek DayOfWeek { get; set; }
    }
}
