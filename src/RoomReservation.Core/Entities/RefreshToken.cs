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

        [Required]
        [MaxLength(100)]
        public string Token { get; set; } = string.Empty;

        [Required]
        public DateTime Created { get; set; } = DateTime.UtcNow;

        [Required]
        public DateTime Expires { get; set; }

        public DateTime? Revoked { get; set; }

        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= Expires;

        [NotMapped]
        public bool IsActive => Revoked == null && !IsExpired;

        [NotMapped]
        public bool IsRevoked => Revoked != null;

        [Required]
        [ForeignKey(nameof(User))]
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
