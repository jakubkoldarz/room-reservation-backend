using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Models
{
    public record SpecialAvailabilitySlot(DateOnly StartDate, DateOnly EndDate, bool IsClosed, TimeOnly? StartTime, TimeOnly? EndTime);
}
