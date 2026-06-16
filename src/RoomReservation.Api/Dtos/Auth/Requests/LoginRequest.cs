using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Users.Responses
{
    public record LoginRequest
    (
        [Required, EmailAddress] string Email,
        [Required] string Password
    );
}
