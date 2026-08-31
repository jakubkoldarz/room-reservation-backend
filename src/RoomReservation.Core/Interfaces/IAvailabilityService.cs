using RoomReservation.Core.Entities;
using RoomReservation.Core.Models.Availability;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Interfaces
{
    public interface IAvailabilityService
    {
        Task<bool> AreAvailabilitiesValid(IReadOnlyList<Availability> availabilities, Guid? boundingBuildingId = null);

        Task<AvailabilityResolution> ResolveAvailabilityAsync(Guid roomId, DateOnly date);

        Task<IReadOnlyList<Availability>> GetAllForRoomAsync(Guid roomId);
        Task<IReadOnlyList<Room>> GetConflictingRoomsAsync(Guid buildingId, IReadOnlyList<Availability> newAvailabilities);

        Task<ResultT<IReadOnlyList<Availability>>> ReplaceForRoomAsync(Guid roomId, IReadOnlyList<AvailabilityModel> request, bool force = false);

        Task<IReadOnlyList<Reservation>> GetConflictingReservationsForRoomAsync(Guid roomId, IReadOnlyList<Availability> availabilities, IReadOnlyList<Event> events);
        Task<IReadOnlyList<Reservation>> GetConflictingReservationsForRoomsAsync(IReadOnlyList<Guid> roomIds, IReadOnlyList<Availability> availabilities, IReadOnlyList<Event> events);
    }
}