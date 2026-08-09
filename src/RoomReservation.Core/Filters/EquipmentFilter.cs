using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Filters
{
    public class EquipmentFilter : PagedFilter
    {
        public string? Name { get; set; }
    }
}
