namespace RoomReservation.Api.Dtos.Buildings.Responses
{
    public class BasicBuildingResponseDto
    {
        public required Guid Id { get; init; }
        public required string Name { get; init; }
        public string? Identifier { get; init; }
        public required string Street { get; init; }
        public required string City { get; init; }
        public required string PostalCode { get; init; }
        public required int FloorsCount { get; init; }
    };
}
