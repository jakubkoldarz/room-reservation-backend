using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Auth.Requests
{
    public class LoginRequestDto
    {
        [Required, EmailAddress] public required string Email { get; init; }
        [Required] public required string Password { get; init; }
    }
}
