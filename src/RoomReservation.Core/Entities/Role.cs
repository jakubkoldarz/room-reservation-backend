namespace RoomReservation.Core.Entities
{
    public class Role
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public required string Name { get; set; }
        public bool IsDefault { get; set; } = false;
        public bool IsSuperAdmin { get; set; } = false;
        public ICollection<RolePermissions> RolePermissions { get; set; } = [];
    }
}
