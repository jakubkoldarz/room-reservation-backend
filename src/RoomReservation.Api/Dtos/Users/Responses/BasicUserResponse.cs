namespace RoomReservation.Api.Dtos.Users.Responses
{
    public record BasicUserResponse
    (
        Guid Id,
        string Firstname, 
        string Lastname
    );
}
