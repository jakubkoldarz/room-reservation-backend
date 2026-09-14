namespace RoomReservation.Api.Dtos.Auth.Responses
{
    public class RoleWithPermissionsResponseDto
    {
        public string Role { get; init; } = string.Empty;
        public string[] Permissions { get; init; } = [];
    }
}
