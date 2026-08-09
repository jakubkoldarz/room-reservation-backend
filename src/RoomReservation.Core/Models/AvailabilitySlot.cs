using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Models
{
    public record AvailabilitySlot(DayOfWeek DayOfWeek, TimeOnly StartTime, TimeOnly EndTime);
}
