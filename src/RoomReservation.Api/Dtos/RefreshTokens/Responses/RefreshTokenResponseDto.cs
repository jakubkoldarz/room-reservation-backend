namespace RoomReservation.Api.Dtos.RefreshTokens.Responses
{
    public class RefreshTokenResponseDto
    {
        public required Guid Id { get; init; }
        public required DateTime Created { get; init; }
        public required DateTime Expires { get; init; }
        public string? IpAddress { get; init; }
        public string? UserAgent { get; init; }
    }
}
