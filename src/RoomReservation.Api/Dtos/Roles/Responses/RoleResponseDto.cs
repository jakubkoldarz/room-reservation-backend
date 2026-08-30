namespace RoomReservation.Api.Dtos.Roles.Responses
{
    public record RoleResponseDto(
        Guid Id,
        string Name,
        bool IsDefault,
        bool IsSuperAdmin,
        IReadOnlyList<string> Permissions
    );
}