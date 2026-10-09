using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Core.Options
{
    public class JwtOptions
    {
        public const string SectionName = "Jwt";

        [Required, MinLength(32)] public string Secret { get; init; } = string.Empty;
        [Required] public string Issuer { get; init; } = string.Empty;
        [Required] public string Audience { get; init; } = string.Empty;
    }
}
