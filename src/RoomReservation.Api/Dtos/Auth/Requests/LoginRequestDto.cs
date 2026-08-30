using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace RoomReservation.Api.Dtos.Auth.Requests
{
    public record LoginRequestDto
    (
        [Required, EmailAddress] string Email,
        [Required] string Password
    );
}
