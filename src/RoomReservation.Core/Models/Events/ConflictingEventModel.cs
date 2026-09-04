using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Models.Events
{
    public record ConflictingEventModel
    (
        Guid EventId,
        string Name,
        DateOnly StartDate,
        DateOnly EndDate
    );
}
