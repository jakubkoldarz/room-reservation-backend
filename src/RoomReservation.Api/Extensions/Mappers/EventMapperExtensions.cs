using RoomReservation.Api.Dtos.Events.Responses;
using RoomReservation.Core.Entities;

namespace RoomReservation.Api.Extensions.Mappers
{
    public static class EventMapperExtensions
    {
        public static EventResponseDto ToDto(this Event ev)
        {
            return new EventResponseDto 
            {
                Id = ev.Id,
                Name = ev.Name,
                StartDate = ev.StartDate,
                EndDate = ev.EndDate,
                IsClosed = ev.IsClosed,
                StartTime = ev.StartTime,
                EndTime = ev.EndTime,
                Rooms = [.. ev.Rooms.Select(r => r.ToBasicDto())]
            };
        }
    }
}
