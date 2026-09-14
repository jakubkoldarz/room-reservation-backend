namespace RoomReservation.Api.Dtos.Roles.Responses
{
    public class RoleResponseDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public bool IsDefault { get; init; }
        public bool IsSuperAdmin { get; init; }
        public IReadOnlyList<string> Permissions { get; init; } = [];
    }
}