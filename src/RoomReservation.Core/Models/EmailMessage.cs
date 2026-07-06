using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Models
{
    public class EmailMessage
    {
        public required string To { get; set; } = string.Empty;
        public required string Subject { get; set; } = string.Empty;
        public required string HtmlMessage { get; set; } = string.Empty; 
    }
}
