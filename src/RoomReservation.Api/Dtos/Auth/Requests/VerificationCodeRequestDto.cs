using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Auth.Requests
{
    public class VerificationCodedRequestDto
    {
        [Required] public required string VerificationCode { get; init; }
    }
}