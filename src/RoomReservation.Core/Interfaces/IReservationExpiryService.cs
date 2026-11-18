namespace RoomReservation.Core.Interfaces
{
    public interface IReservationExpiryService
    {
        Task<int> ExpireStartedRequestsAsync();
    }
}
