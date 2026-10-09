namespace RoomReservation.Api.Dtos.Roles.Responses
{
    public class RoleResponseDto
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; } = string.Empty;
        public required bool IsDefault { get; init; }
        public required bool IsSuperAdmin { get; init; }
        public required IReadOnlyList<string> Permissions { get; init; } = [];
    }
}