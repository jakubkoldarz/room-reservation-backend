using RoomReservation.Api.Dtos.Roles.Responses;
using RoomReservation.Core.Entities;

namespace RoomReservation.Api.Extensions.Mappers
{
    public static class RoleMapperExtensions
    {
        public static RoleResponseDto ToDto(this Role role)
        {
            return new RoleResponseDto(
                Id: role.Id, 
                Name: role.Name, 
                IsDefault: role.IsDefault, 
                IsSuperAdmin: role.IsSuperAdmin,
                Permissions: [.. role.RolePermissions.Select(rp => rp.Permission.Name)]);
        }
    }
}
