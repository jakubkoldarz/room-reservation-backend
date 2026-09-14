namespace RoomReservation.Api.Dtos.RefreshTokens.Responses
{
    public class RefreshTokenResponseDto
    {
        public Guid Id { get; init; }
        public DateTime Created { get; init; }
        public DateTime Expires { get; init; }
        public string? IpAddress { get; init; }
        public string? UserAgent { get; init; }
    }
}
