using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Auth.Requests
{
    public record EmailRequestDto(
        [Required, EmailAddress] string EmailAddress
    );
}
