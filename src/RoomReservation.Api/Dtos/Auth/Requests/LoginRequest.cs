using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Auth.Requests
{
    public record LoginRequest
    (
        [Required, EmailAddress] string Email,
        [Required] string Password
    );
}
