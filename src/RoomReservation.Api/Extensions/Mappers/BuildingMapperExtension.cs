using RoomReservation.Api.Dtos.Buildings.Responses;
using RoomReservation.Core.Entities;

namespace RoomReservation.Api.Extensions.Mappers
{
    public static class BuildingMapperExtension
    {
        public static BasicBuildingResponse ToBasicDto(this Building building)
        {
            return new BasicBuildingResponse
            (
                building.Id,
                building.Name,
                building.Identifier,
                building.Street,
                building.City,
                building.PostalCode,
                building.FloorsCount
            );
        }

        public static BuildingDetailsResponse ToDetailsDto(this Building building)
        {
            return new BuildingDetailsResponse
            (
                building.ToBasicDto(),
                [.. building.Rooms.Select(r => r.ToBasicDto())]
            );
        }
    }
}
