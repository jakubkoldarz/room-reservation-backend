using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Users.Requests
{
    public record UpdateProfileRequestDto
    (
        [Required, MinLength(3), MaxLength(50)] string Firstname,
        [Required, MaxLength(100)] string Lastname
    );
}