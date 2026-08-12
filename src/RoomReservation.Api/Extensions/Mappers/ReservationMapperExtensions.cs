using RoomReservation.Api.Dtos.Reservations.Responses;
using RoomReservation.Core.Entities;

namespace RoomReservation.Api.Extensions.Mappers
{
    public static class ReservationMapperExtensions
    {
        public static BasicReservationResponse ToBasicDto(this Reservation reservation)
        {
            return new BasicReservationResponse(
        
            );
        }
    }
}
