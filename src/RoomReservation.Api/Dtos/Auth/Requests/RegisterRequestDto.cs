using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Auth.Requests
{
    public record RegisterRequestDto
    (
        [Required, EmailAddress] string Email,
        [Required, MinLength(8)] string Password
    );
}
