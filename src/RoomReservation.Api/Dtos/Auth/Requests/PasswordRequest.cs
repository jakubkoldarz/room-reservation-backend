using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Auth.Requests
{
    public record ChangePasswordRequest
    (
        [Required, MinLength(8)] string OldPassword,
        [Required, MinLength(8)] string NewPassword
    );
}
