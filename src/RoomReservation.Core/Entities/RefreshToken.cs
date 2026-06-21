using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RoomReservation.Core.Entities
{
    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; } = Guid.CreateVersion7();

        [MaxLength(100)]
        public required string TokenHash { get; set; } = string.Empty;
        public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public required DateTime ExpiresAt { get; set; }
        public DateTime? RevokedAt { get; set; }

        [MaxLength(30)]
        public string? IpAddress { get; set; }
        [MaxLength(500)]
        public string? UserAgent { get; set; }

        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
        [NotMapped]
        public bool IsActive => RevokedAt == null && !IsExpired;
        [NotMapped]
        public bool IsRevoked => RevokedAt != null;

        [ForeignKey(nameof(User))]
        public required Guid UserId { get; set; }
        public User User { get; set; } = null!;
        public Guid? ReplacedByTokenId { get; set; }
    }
}
