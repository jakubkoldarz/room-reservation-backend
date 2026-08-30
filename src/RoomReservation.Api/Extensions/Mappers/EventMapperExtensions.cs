using RoomReservation.Api.Dtos.Events.Responses;
using RoomReservation.Core.Entities;

namespace RoomReservation.Api.Extensions.Mappers
{
    public static class EventMapperExtensions
    {
        public static EventResponseDto ToDto(this Event ev)
        {
            return new EventResponseDto(
                ev.Id, ev.Name, ev.StartDate, ev.EndDate, ev.IsClosed, ev.StartTime, ev.EndTime,
                [.. ev.Rooms.Select(r => r.Id)]);
        }
    }
}
