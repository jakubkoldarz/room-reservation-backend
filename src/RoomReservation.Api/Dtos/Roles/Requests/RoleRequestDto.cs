using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Roles.Requests
{
    public class RoleRequestDto
    {
        [Required, MaxLength(50)] public string Name { get; init; } = string.Empty;
        [Required] public bool IsDefault { get; init; }
        [Required] public bool IsSuperAdmin { get; init; }
        [Required] public IReadOnlyList<Guid> PermissionIds { get; init; } = [];
    }
}