using RoomReservation.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RoomReservation.Core.Entities
{
    public class VerificationCode
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();

        public required Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public required string Code { get; set; }
        public required VerificationCodeType Type { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public required DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; } = false;
    }
}
