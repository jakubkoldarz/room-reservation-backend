namespace RoomReservation.Api.Dtos.Buildings.Responses
{
    public record BasicBuildingResponse(
        Guid Id,
        string Name,
        string? Identifier,
        string Street,
        string City,
        string PostalCode,
        int FloorsCount
    );
}
