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
                Id: building.Id,
                Name: building.Name,
                Identifier: building.Identifier,
                Street: building.Street,
                City: building.City,
                PostalCode: building.PostalCode,
                FloorsCount: building.FloorsCount
            );
        }

        public static BuildingDetailsResponse ToDetailsDto(this Building building)
        {
            return new BuildingDetailsResponse
            (
                BuildingInfo: building.ToBasicDto(),
                Availabilities: [.. building.Availabilities.Select(ba => ba.ToDto())],
                SpecialAvailabilities: [.. building.SpecialAvailabilities.Select(sa => sa.ToDto())],
                Rooms: [.. building.Rooms.Select(r => r.ToBasicDto())]
            );
        }
    }
}
