namespace RoomReservation.Core.Constants
{
    public static class RolesPermissions
    {
        public static Dictionary<RoleDefinition, string[]> All = new()
        {
            [Roles.User] = [
                Permissions.RoomView, Permissions.RoomList, 
                Permissions.UserView, Permissions.UserList,
                Permissions.BuildingView, Permissions.BuildingList,
                Permissions.ReservationView, Permissions.ReservationCreate
            ],

            [Roles.Receptionist] = [
                Permissions.RoomView, Permissions.RoomList,
                Permissions.UserView, Permissions.UserList,
                Permissions.BuildingView, Permissions.BuildingList,
                Permissions.ReservationView, Permissions.ReservationList,
                Permissions.ReservationApprove, Permissions.ReservationReject, Permissions.ReservationForceCancel
            ],

            [Roles.Manager] = [
                Permissions.RoomView, Permissions.RoomList, Permissions.RoomEdit, Permissions.RoomAdd,
                Permissions.UserView, Permissions.UserList,
                Permissions.BuildingView, Permissions.BuildingList, Permissions.BuildingEdit,
                Permissions.ReservationView, Permissions.ReservationList,
                Permissions.ReservationApprove, Permissions.ReservationReject, Permissions.ReservationForceCancel
            ],

            [Roles.SuperAdmin] = [ /* ALL PERMISSIONS */ ]
        };
    }
}
