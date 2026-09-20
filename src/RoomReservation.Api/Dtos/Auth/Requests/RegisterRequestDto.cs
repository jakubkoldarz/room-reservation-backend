using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Auth.Requests
{
    public class RegisterRequestDto
    {
        [Required, EmailAddress] public required string Email { get; init; }
        [Required, MinLength(8)] public required string Password { get; init; }
    }
}
