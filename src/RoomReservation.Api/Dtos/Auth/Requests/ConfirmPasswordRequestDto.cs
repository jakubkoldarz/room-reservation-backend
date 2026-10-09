using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Auth.Requests
{
    public class ConfirmPasswordRequestDto
    {
        [Required] public required string Password { get; init; }
    }
}
