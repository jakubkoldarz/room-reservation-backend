namespace RoomReservation.Api.Dtos.Auth.Responses
{
    public record RoleWithPermissionsResponseDto(
        string Role,
        string[] Permissions
    );
}
