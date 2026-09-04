using RoomReservation.Api.Dtos.Availabilities.Responses;
using RoomReservation.Core.Entities;

namespace RoomReservation.Api.Extensions.Mappers
{
    public static class AvailabilityMapperExtensions
    {
        public static AvailabilityResponseDto ToDto(this Availability availability)
        {
            return new AvailabilityResponseDto
            (
                DayOfWeek: availability.DayOfWeek,
                StartTime: availability.StartTime,
                EndTime: availability.EndTime
            );
        }
    }
}
