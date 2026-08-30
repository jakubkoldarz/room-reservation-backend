namespace RoomReservation.Core.Models
{
    public record RoleModel(
        string Name,
        string Description,
        bool IsDefault,
        bool IsSuperAdmin,
        IReadOnlyList<Guid> PermissionIds
    );
}
