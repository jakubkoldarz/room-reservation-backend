using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Auth.Requests
{
    public class ResetPasswordRequestDto
    {
        [Required, EmailAddress] public required string EmailAddress { get; init; }
        [Required] public required string VerificationCode { get; init; }
        [Required, MinLength(8)] public required string NewPassword { get; init; }
    }
}
