using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Auth.Requests
{
    public record RegisterRequest
    (
        [Required, EmailAddress] string Email,
        [Required, MinLength(8)] string Password
    );
}
