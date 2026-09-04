using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Api.Dtos.Roles.Requests
{
    public record RoleRequestDto(
        [Required, MaxLength(50)] string Name,
        [MaxLength(200)] string? Description,
        [Required] bool IsDefault,
        [Required] bool IsSuperAdmin,
        [Required] IReadOnlyList<Guid> PermissionIds
    );
}