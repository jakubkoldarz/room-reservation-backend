namespace RoomReservation.Core.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }

        public required string Email { get; set; }
        public string? PendingEmail { get; set; }

        public required string PasswordHash { get; set; }
        public bool IsProfileComplete { get; set; }
        public bool IsEmailVerified { get; set; }
        public bool Is2faEnabled { get; set; }

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new HashSet<RefreshToken>();

        public Guid RoleId { get; set; }
        public Role Role { get; set; } = null!;

        public ICollection<Reservation> Reservations { get; set; } = [];
    }
}
