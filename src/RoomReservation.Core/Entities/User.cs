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
        public required string Firstname { get; set; } 

        [MaxLength(100)]
        public required string Lastname { get; set; } 

        [EmailAddress]
        public required string Email { get; set; } 

        public required string PasswordHash { get; set; }

        public IEnumerable<RefreshToken> RefreshTokens { get; set; } = [];
    }
}
