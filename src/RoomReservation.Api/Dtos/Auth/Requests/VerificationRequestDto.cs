using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Auth.Requests
{
    public class VerificationRequestDto
    {
        [Required] public required Guid VerificationId { get; init; }
        [Required] public required string VerificationCode { get; init; }
    }
}