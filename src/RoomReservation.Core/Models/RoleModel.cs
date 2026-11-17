namespace RoomReservation.Core.Models
{
    public record RoleModel(
        string Name,
        bool IsDefault,
        bool IsSuperAdmin,
        IReadOnlyList<Guid> PermissionIds
    );
}
