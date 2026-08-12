using RoomReservation.Api.Dtos.Availabilities.Responses;
using RoomReservation.Core.Entities;

namespace RoomReservation.Api.Extensions.Mappers
{
    public static class AvailabilityMapperExtensions
    {
        public static AvailabilityResponse ToDto(this RoomAvailability availability)
        {
            return new AvailabilityResponse
            (
                DayOfWeek: availability.DayOfWeek,
                StartTime: availability.StartTime,
                EndTime: availability.EndTime
            );
        }

        public static SpecialAvailabilityResponse ToDto(this RoomSpecialAvailability availability)
        {
            return new SpecialAvailabilityResponse
            (
                Id: availability.Id,
                StartDate: availability.StartDate,
                EndDate: availability.EndDate,
                IsClosed: availability.IsClosed,
                StartTime: availability.StartTime,
                EndTime: availability.EndTime
            );
        }

        public static AvailabilityResponse ToDto(this BuildingAvailability availability)
        {
            return new AvailabilityResponse
            (
                DayOfWeek: availability.DayOfWeek,
                StartTime: availability.StartTime,
                EndTime: availability.EndTime
            );
        }

        public static SpecialAvailabilityResponse ToDto(this BuildingSpecialAvailability availability)
        {
            return new SpecialAvailabilityResponse
            (
                Id: availability.Id,
                StartDate: availability.StartDate,
                EndDate: availability.EndDate,
                IsClosed: availability.IsClosed,
                StartTime: availability.StartTime,
                EndTime: availability.EndTime
            );
        }
    }
}
