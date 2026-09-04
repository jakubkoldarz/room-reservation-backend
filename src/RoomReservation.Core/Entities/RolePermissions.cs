namespace RoomReservation.Core.Entities
{
    public class RolePermissions
    {
        public required Guid RoleId { get; set; }
        public required Guid PermissionId { get; set; }

        public Role Role { get; set; } = null!;
        public Permission Permission { get; set; } = null!;
    }
}
