using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Auth.Requests
{
    public record VerificationRequest
    (
        [Required] Guid VerificationId,
        [Required] string VerificationCode
    );
}