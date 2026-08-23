namespace RoomReservation.Core.Constants
{
    public static class Permissions
    {
        public const string RoomView = "room.view";
        public const string RoomList = "room.list";
        public const string RoomAdd = "room.add";
        public const string RoomDelete = "room.delete";
        public const string RoomEdit = "room.edit";
        public const string RoomEditAvailability = "room.edit.availability";

        public const string UserView = "user.view";
        public const string UserList = "user.list";
        public const string UserBlock = "user.block";

        public const string BuildingView = "building.view";
        public const string BuildingList = "building.list";
        public const string BuildingAdd = "building.add";
        public const string BuildingDelete = "building.delete";
        public const string BuildingEdit = "building.edit";
        public const string BuildingEditAvailability = "building.edit.availability";

        public const string EquipmentView = "equipment.view";
        public const string EquipmentList = "equipment.list";
        public const string EquipmentAdd = "equipment.add";
        public const string EquipmentDelete = "equipment.delete";
        public const string EquipmentEdit = "equipment.edit";

        public const string ReservationView = "reservation.view";
        public const string ReservationList = "reservation.list";
        public const string ReservationForceCancel = "reservation.force.cancel";
        public const string ReservationApprove = "reservation.approve";
        public const string ReservationReject = "reservation.reject";
        public const string ReservationCreate = "reservation.create";

        public static Dictionary<string, Guid> Definitions = new()
        {
            [RoomView] = Guid.Parse("10000000-0000-0000-0000-000000000000"),
            [RoomList] = Guid.Parse("10000000-0000-0000-0000-000000000001"),
            [RoomAdd] = Guid.Parse("10000000-0000-0000-0000-000000000002"),
            [RoomDelete] = Guid.Parse("10000000-0000-0000-0000-000000000003"),
            [RoomEdit] = Guid.Parse("10000000-0000-0000-0000-000000000004"),
            [RoomEditAvailability] = Guid.Parse("10000000-0000-0000-0000-000000000005"),

            [UserView] = Guid.Parse("20000000-0000-0000-0000-000000000000"),
            [UserList] = Guid.Parse("20000000-0000-0000-0000-000000000001"),

            [BuildingAdd] = Guid.Parse("30000000-0000-0000-0000-000000000000"),
            [BuildingList] = Guid.Parse("30000000-0000-0000-0000-000000000001"),
            [BuildingAdd] = Guid.Parse("30000000-0000-0000-0000-000000000002"),
            [BuildingDelete] = Guid.Parse("30000000-0000-0000-0000-000000000003"),
            [BuildingEdit] = Guid.Parse("30000000-0000-0000-0000-000000000004"),
            [BuildingEditAvailability] = Guid.Parse("30000000-0000-0000-0000-000000000005"),

            [EquipmentView] = Guid.Parse("40000000-0000-0000-0000-000000000000"),
            [EquipmentList] = Guid.Parse("40000000-0000-0000-0000-000000000001"),
            [EquipmentAdd] = Guid.Parse("40000000-0000-0000-0000-000000000002"),
            [EquipmentDelete] = Guid.Parse("40000000-0000-0000-0000-000000000003"),
            [EquipmentEdit] = Guid.Parse("40000000-0000-0000-0000-000000000004"),

            [ReservationView] = Guid.Parse("50000000-0000-0000-0000-000000000000"),
            [ReservationList] = Guid.Parse("50000000-0000-0000-0000-000000000001"),
            [ReservationForceCancel] = Guid.Parse("50000000-0000-0000-0000-000000000002"),
            [ReservationApprove] = Guid.Parse("50000000-0000-0000-0000-000000000003"),
            [ReservationReject] = Guid.Parse("50000000-0000-0000-0000-000000000004"),
            [ReservationCreate] = Guid.Parse("50000000-0000-0000-0000-000000000005"),
        };
    }
}
