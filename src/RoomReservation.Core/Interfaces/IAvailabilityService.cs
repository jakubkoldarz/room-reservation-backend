using RoomReservation.Core.Entities;
using RoomReservation.Core.Models.Availability;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Interfaces
{
    public interface IAvailabilityService
    {
        Task<Result> AreAvailabilitiesValid(IReadOnlyList<Availability> availabilities, Guid? boundingBuildingId = null);

        Task<AvailabilityResolution> ResolveAvailabilityAsync(Guid roomId, DateOnly date);

        Task<IReadOnlyList<Availability>> GetAllForRoomAsync(Guid roomId);
        Task<IReadOnlyList<Room>> GetConflictingRoomsAsync(Guid buildingId, IReadOnlyList<Availability> newAvailabilities);

        Task<ResultT<IReadOnlyList<Reservation>>> ReplaceIfValidForRoomAsync(Room room, IReadOnlyList<AvailabilityModel> models, bool force = false);
        Task<ResultT<IReadOnlyList<Availability>>> ReplaceIfValidForBuildingAsync(Building building, IReadOnlyList<AvailabilityModel> models);

        Task<IReadOnlyList<Reservation>> GetConflictingReservationsForRoomAsync(Guid roomId, IReadOnlyList<Availability> availabilities, IReadOnlyList<Event> events);
        Task<IReadOnlyList<Reservation>> GetConflictingReservationsForRoomsAsync(IReadOnlyList<Guid> roomIds, IReadOnlyList<Availability> availabilities, IReadOnlyList<Event> events);
    }
}