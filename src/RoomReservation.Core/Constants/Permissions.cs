using System.Collections.ObjectModel;

namespace RoomReservation.Core.Constants
{
    public static class Permissions
    {
        public const string RoomView = "room.view";
        public const string RoomList = "room.list";
        public const string RoomAdd = "room.add";
        public const string RoomDelete = "room.delete";
        public const string RoomEdit = "room.edit";

        public const string UserView = "user.view";
        public const string UserList = "user.list";
        public const string UserBlock = "user.block";

        public static Dictionary<string, Guid> Definitions = new()
        {
            [RoomView] = Guid.Parse("10000000-0000-0000-0000-000000000000"),
            [RoomList] = Guid.Parse("10000000-0000-0000-0000-000000000001"),
            [RoomAdd] = Guid.Parse("10000000-0000-0000-0000-000000000002"),
            [RoomDelete] = Guid.Parse("10000000-0000-0000-0000-000000000003"),
            [RoomEdit] = Guid.Parse("10000000-0000-0000-0000-000000000004"),

            [UserView] = Guid.Parse("20000000-0000-0000-0000-000000000000"),
            [UserList] = Guid.Parse("20000000-0000-0000-0000-000000000001"),
        };
    }
}
