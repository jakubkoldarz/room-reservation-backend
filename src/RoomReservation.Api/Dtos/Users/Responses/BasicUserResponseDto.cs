namespace RoomReservation.Api.Dtos.Users.Responses
{
    public record BasicUserResponseDto
    (
        Guid Id,
        string? Firstname, 
        string? Lastname
    );
}
