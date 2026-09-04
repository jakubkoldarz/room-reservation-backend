namespace RoomReservation.Core.Entities
{
    public class Permission
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public required string Name { get; set; }
        public ICollection<RolePermissions> RolePermissions { get; set; } = [];
    }
}
