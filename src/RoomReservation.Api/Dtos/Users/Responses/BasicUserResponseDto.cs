namespace RoomReservation.Api.Dtos.Users.Responses
{
    public class BasicUserResponseDto
    {
        public required Guid Id { get; init; }
        public string? Firstname { get; init; }
        public string? Lastname { get; init; }
    }
}
