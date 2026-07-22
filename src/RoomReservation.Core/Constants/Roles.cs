namespace RoomReservation.Core.Constants
{
    public record RoleDefinition(Guid Id, string Name, bool IsSuperAdmin = false, bool IsDefault = false);

    public static class Roles
    {
        public static readonly RoleDefinition User = new(Guid.Parse("10000000-0000-0000-0000-000000000000"), "User", IsDefault: true);
        public static readonly RoleDefinition Receptionist = new(Guid.Parse("10000000-0000-0000-0000-000000000001"), "Receptionist");
        public static readonly RoleDefinition Manager = new(Guid.Parse("10000000-0000-0000-0000-000000000002"), "Manager");
        public static readonly RoleDefinition SuperAdmin = new(Guid.Parse("10000000-0000-0000-0000-000000000003"), "SuperAdmin", IsSuperAdmin: true);
    }
}
