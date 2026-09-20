using RoomReservation.Api.Dtos.Permissions.Responses;
using RoomReservation.Core.Entities;

namespace RoomReservation.Api.Extensions.Mappers
{
    public static class PermissionMapperExtensions
    {
        public static PermissionResponseDto ToDto(this Permission permission)
        {
            return new()
            {
                Id = permission.Id,
                Name = permission.Name
            };
        }
    }
}
