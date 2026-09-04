using RoomReservation.Api.Dtos.Buildings.Responses;
using RoomReservation.Core.Entities;

namespace RoomReservation.Api.Extensions.Mappers
{
    public static class BuildingMapperExtension
    {
        public static BasicBuildingResponseDto ToBasicDto(this Building building)
        {
            return new BasicBuildingResponseDto
            (
                Id: building.Id,
                Name: building.Name,
                Identifier: building.Identifier,
                Street: building.Street,
                City: building.City,
                PostalCode: building.PostalCode,
                FloorsCount: building.FloorsCount
            );
        }

        public static BuildingDetailsResponseDto ToDetailsDto(this Building building)
        {
            return new BuildingDetailsResponseDto
            (
                BuildingInfo: building.ToBasicDto(),
                Availabilities: [.. building.Availabilities.Select(ba => ba.ToDto())],
                Rooms: [.. building.Rooms.Select(r => r.ToBasicDto())]
            );
        }
    }
}
