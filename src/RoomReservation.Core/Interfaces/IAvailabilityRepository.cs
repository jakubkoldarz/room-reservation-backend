using RoomReservation.Core.Entities;

namespace RoomReservation.Core.Interfaces
{
    public interface IAvailabilityRepository
    {
        Task<IReadOnlyList<Availability>> GetByRoomAsync(Guid roomId);
        Task<IReadOnlyList<Availability>> GetByRoomIdsAsync(IReadOnlyList<Guid> roomIds);
        Task<IReadOnlyList<Availability>> GetByBuildingAsync(Guid buildingId);
        Task ReplaceForRoomAsync(Guid roomId, IReadOnlyList<Availability> availabilities); 
    }
}
