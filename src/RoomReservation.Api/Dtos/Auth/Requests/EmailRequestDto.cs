using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Auth.Requests
{
    public class EmailRequestDto
    {
        [Required, EmailAddress] public required string EmailAddress { get; init; }
    }
}
