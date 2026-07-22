namespace RoomReservation.Core.Constants
{
    public record PermissionDefinition(string Name, Guid Id);

    public static class Permissions
    {
        public static readonly PermissionDefinition RoomView = new("room.view", Guid.Parse("10000000-0000-0000-0000-000000000000"));
        public static readonly PermissionDefinition RoomList = new("room.list", Guid.Parse("10000000-0000-0000-0000-000000000001"));
        public static readonly PermissionDefinition RoomAdd = new("room.add", Guid.Parse("10000000-0000-0000-0000-000000000002"));
        public static readonly PermissionDefinition RoomDelete = new("room.delete", Guid.Parse("10000000-0000-0000-0000-000000000003"));
        public static readonly PermissionDefinition RoomEdit = new("room.edit", Guid.Parse("10000000-0000-0000-0000-000000000004"));

        public static readonly PermissionDefinition UserView = new("user.view", Guid.Parse("20000000-0000-0000-0000-000000000000"));
        public static readonly PermissionDefinition UserList = new("user.list", Guid.Parse("20000000-0000-0000-0000-000000000001"));
        public static readonly PermissionDefinition UserBlock = new("user.block", Guid.Parse("20000000-0000-0000-0000-000000000002"));
    }
}
