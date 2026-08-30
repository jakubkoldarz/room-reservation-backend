using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Auth.Requests
{
    public record VerificationRequestDto
    (
        [Required] Guid VerificationId,
        [Required] string VerificationCode
    );
}