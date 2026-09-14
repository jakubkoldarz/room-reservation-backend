using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Auth.Requests
{
    public class ChangePasswordRequestDto
    {
        [Required, MinLength(8)] public required string OldPassword { get; init; }
        [Required, MinLength(8)] public required string NewPassword { get; init; }
    }
}
