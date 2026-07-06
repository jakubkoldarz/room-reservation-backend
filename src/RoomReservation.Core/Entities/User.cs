using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RoomReservation.Core.Entities
{
    public class User
    {
        [Key]
        public Guid Id { get; set; } = Guid.CreateVersion7();

        [MaxLength(50)]
        public string? Firstname { get; set; }

        [MaxLength(100)]
        public string? Lastname { get; set; }

        [EmailAddress]
        public required string Email { get; set; } 

        public required string PasswordHash { get; set; }
        public bool IsProfileComplete { get; set; } = false;
        public bool IsEmailVerified { get; set; } = false;
        public bool Is2faEnabled { get; set; } = false;

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();
    }
}
