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
                BuildingInfo: room.Building.ToBasicDto(),
                Floor: room.Floor,
                Capacity: room.Capacity,
                Equipment: [.. room.RoomEquipment.Select(re => re.Equipment.ToBasicDto())]
            );
        }

        public static RoomDetailsResponseDto ToDetailsDto(this Room room)
        {
            return new RoomDetailsResponseDto
            (
                RoomInfo: room.ToBasicDto(),
                Availabilities: [.. room.Availabilities.Select(a => a.ToDto())],
                Reservations: [.. room.Reservations.Select(r => r.ToBasicDto())]
            );
        }
    }
}