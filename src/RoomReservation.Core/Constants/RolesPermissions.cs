using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Constants
{
    public static class RolesPermissions
    {
        public static Dictionary<RoleDefinition, PermissionDefinition[]> All = new()
        {
            [Roles.User] = [Permissions.RoomView, Permissions.RoomList],
            [Roles.Receptionist] = [Permissions.RoomView, Permissions.RoomList],
            [Roles.Manager] = [Permissions.RoomView, Permissions.RoomList, Permissions.RoomAdd, Permissions.RoomEdit, Permissions.RoomDelete],
            [Roles.SuperAdmin] = []
        };
    }
}
