using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Reservations.Requests
{
    public record UpdateReservationRequestDto
    (
        [Required] TimeOnly StartTime,
        [Required] TimeOnly EndTime,
        [MaxLength(100)] string? Purpose    
    );
}
