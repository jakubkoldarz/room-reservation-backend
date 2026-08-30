using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Auth.Requests
{
    public record VerificationCodedRequestDto
    (
        [Required] string VerificationCode
    );
}