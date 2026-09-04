namespace RoomReservation.Api.Dtos.Buildings.Responses
{
    public record BasicBuildingResponseDto(
        Guid Id,
        string Name,
        string? Identifier,
        string Street,
        string City,
        string PostalCode,
        int FloorsCount
    );
}
