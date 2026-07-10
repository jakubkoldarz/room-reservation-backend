using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Auth.Requests
{
    public record EmailRequest(
        [Required, EmailAddress] string EmailAddress
    );
}
