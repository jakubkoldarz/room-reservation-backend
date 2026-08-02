using RoomReservation.Api.Dtos.Buildings.Responses;
using RoomReservation.Api.Dtos.Rooms.Responses;
using RoomReservation.Core.Entities;

namespace RoomReservation.Api.Extensions.Mappers
{
    public static class RoomMapperExtensions
    {
        public static BasicRoomResponse ToBasicDto(this Room room)
        {
            return new BasicRoomResponse();
        }
    }
}