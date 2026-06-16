using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Auth.Requests
{
    public record RegisterRequest
    (
        [Required, EmailAddress] string Email,
        [Required] string Password,
        [Required, MaxLength(50)] string Firstname,
        [Required, MaxLength(100)] string Lastname
    );
}
