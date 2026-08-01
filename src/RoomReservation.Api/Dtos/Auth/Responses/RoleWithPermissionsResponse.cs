namespace RoomReservation.Api.Dtos.Auth.Responses
{
    public record RoleWithPermissionsResponse(
        string Role,
        string[] Permissions
    );
}
