namespace RoomReservation.Api.Dtos.Users.Responses
{
    public class UserListItemResponseDto
    {
        public required BasicUserResponseDto UserInfo { get; init; }
        public required string Email { get; init; }
        public required UserRoleResponseDto RoleInfo { get; init; }
    }
}
