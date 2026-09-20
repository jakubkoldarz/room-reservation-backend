using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Users.Requests
{
    public class UpdateProfileRequestDto
    {
        [Required, MinLength(3), MaxLength(50)] public required string Firstname { get; init; }
        [Required, MaxLength(100)] public required string Lastname { get; init; } 
    }
}