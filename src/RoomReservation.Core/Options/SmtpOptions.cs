using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Core.Options
{
    public class SmtpOptions
    {
        public const string SectionName = "SMTP";

        [Required] public string Host { get; init; } = string.Empty;
        [Range(1, 65535)] public int Port { get; init; }
        [Required] public string Email { get; init; } = string.Empty;
        [Required] public string Password { get; init; } = string.Empty;
    }
}
