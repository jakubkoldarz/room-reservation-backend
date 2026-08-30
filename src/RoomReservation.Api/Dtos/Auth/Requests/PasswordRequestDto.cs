using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Auth.Requests
{
    public record ChangePasswordRequestDto
    (
        [Required, MinLength(8)] string OldPassword,
        [Required, MinLength(8)] string NewPassword
    );
}
