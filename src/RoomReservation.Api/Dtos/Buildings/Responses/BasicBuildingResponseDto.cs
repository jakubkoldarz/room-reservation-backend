namespace RoomReservation.Api.Dtos.Buildings.Responses
{
    public class BasicBuildingResponseDto
    {
        public Guid Id { get; init; }
        public string Name { get; init; } = string.Empty;
        public string? Identifier { get; init; }
        public string Street { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string PostalCode { get; init; } = string.Empty;
        public int FloorsCount { get; init; }
    };
}
