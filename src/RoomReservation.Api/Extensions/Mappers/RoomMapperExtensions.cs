using RoomReservation.Api.Dtos.Buildings.Responses;
using RoomReservation.Api.Dtos.Rooms.Responses;
using RoomReservation.Core.Entities;

namespace RoomReservation.Api.Extensions.Mappers
{
    public static class RoomMapperExtensions
    {
        public static BasicRoomResponse ToBasicDto(this Room room)
        {
            return new BasicRoomResponse
            (
                Id: room.Id,
                Identifier: room.Identifier,
                RequiresApproval: room.RequiresApproval,
                BuildingInfo: room.Building.ToBasicDto(),
                Floor: room.Floor,
                Capacity: room.Capacity,
                Equipment: [.. room.RoomEquipment.Select(re => re.Equipment.ToBasicDto())],
                Availability: [.. room.RoomAvailabilities.Select(a => a.ToBasicDto())]
            );
        }

        public static RoomAvailabilityResponse ToBasicDto(this RoomAvailability availability)
        {
            return new RoomAvailabilityResponse
            (
                DayOfWeek: availability.DayOfWeek,
                StartTime: availability.StartTime,
                EndTime: availability.EndTime
            );
        }
    }
}