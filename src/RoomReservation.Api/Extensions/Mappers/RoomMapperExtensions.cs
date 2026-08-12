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
                Equipment: [.. room.RoomEquipment.Select(re => re.Equipment.ToBasicDto())]
            );
        }

        public static RoomDetailsResponse ToDetailsDto(this Room room)
        {
            return new RoomDetailsResponse
            (
                RoomInfo: room.ToBasicDto(),
                Availabilities: [.. room.Availabilities.Select(a => a.ToDto())],
                SpecialAvailabilities: [.. room.SpecialAvailabilities.Select(sa => sa.ToDto())],
                Reservations: [.. room.Reservations.Select(r => r.ToBasicDto())]
            );
        }
    }
}