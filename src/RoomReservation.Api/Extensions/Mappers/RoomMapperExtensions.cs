using RoomReservation.Api.Dtos.Rooms.Responses;
using RoomReservation.Core.Entities;

namespace RoomReservation.Api.Extensions.Mappers
{
    public static class RoomMapperExtensions
    {
        public static BasicRoomResponseDto ToBasicDto(this Room room)
        {
            return new BasicRoomResponseDto
            (
                Id: room.Id,
                Identifier: room.Identifier,
                RequiresApproval: room.RequiresApproval,
                Floor: room.Floor,
                Capacity: room.Capacity
            );
        }

        public static RoomDetailsResponseDto ToDetailsDto(this Room room)
        {
            return new RoomDetailsResponseDto
            (
                Details: room.ToDto(),
                Availabilities: [.. room.Availabilities.Select(a => a.ToDto())],
                Reservations: [.. room.Reservations.Select(r => r.ToBasicDto())],
                Events: [.. room.Events.Select(e => e.ToDto())]
            );
        }

        public static RoomResponseDto ToDto(this Room room)
        {
            return new RoomResponseDto
            (
                RoomInfo: room.ToBasicDto(),
                BuildingInfo: room.Building.ToBasicDto(),
                Equipment: [.. room.RoomEquipment.Select(re => re.Equipment.ToBasicDto())]
            );
        }
    }
}