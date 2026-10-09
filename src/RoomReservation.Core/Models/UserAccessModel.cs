namespace RoomReservation.Core.Models
{
    public record UserAccessModel(bool IsProfileComplete, bool IsSuperAdmin, IReadOnlySet<string> Permissions)
    {
        public bool HasPermission(string permission)
            => IsSuperAdmin || Permissions.Contains(permission);
    }
}
