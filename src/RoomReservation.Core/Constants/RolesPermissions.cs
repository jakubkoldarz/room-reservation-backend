namespace RoomReservation.Core.Constants
{
    public static class RolesPermissions
    {
        public static Dictionary<RoleDefinition, string[]> All = new()
        {
            [Roles.User] = [Permissions.RoomView, Permissions.RoomList, Permissions.UserView, Permissions.UserList],
            [Roles.Receptionist] = [Permissions.RoomView, Permissions.RoomList, Permissions.UserView, Permissions.UserList],
            [Roles.Manager] = [Permissions.RoomView, Permissions.RoomList, Permissions.RoomAdd, Permissions.RoomEdit, Permissions.RoomDelete],
            [Roles.SuperAdmin] = []
        };
    }
}
