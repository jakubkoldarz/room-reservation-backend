namespace RoomReservation.Api.Dtos.Auth.Responses
{
    public class RoleWithPermissionsResponseDto
    {
        public required string Role { get; init; }
        public required string[] Permissions { get; init; } = [];
    }
}
