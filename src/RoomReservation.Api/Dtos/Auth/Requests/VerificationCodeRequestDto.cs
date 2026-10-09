using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Auth.Requests
{
    public class VerificationCodeRequestDto
    {
        [Required] public required string VerificationCode { get; init; }
    }
}