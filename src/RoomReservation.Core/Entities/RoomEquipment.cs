using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Entities
{
    public class RoomEquipment
    {
        public Guid RoomId { get; set; }
        public Guid EquipmentId { get; set; }

        public Equipment Equipment { get; set; } = null!;
        public Room Room { get; set; } = null!;
    }
}