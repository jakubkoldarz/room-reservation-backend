using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Auth.Requests
{
    public record VerificationCodedRequest
    (
        [Required] string VerificationCode
    );
}