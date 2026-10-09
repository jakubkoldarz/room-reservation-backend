using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Models;

namespace RoomReservation.Core.Interfaces
{
    public interface IReservationRepository
    {
        Task<Reservation?> GetByIdAsync(Guid reservationId);
        Task<IReadOnlyList<Reservation>> GetByIdsAsync(IReadOnlyList<Guid> reservationIds);
        Task<PagedList<Reservation>> GetFilteredAsync(ReservationFilter filters);
        Task<IReadOnlyList<Reservation>> GetActiveByRoomAndDateAsync(Guid roomId, DateOnly date);
        Task LockRoomAsync(Guid roomId);
        void Add(Reservation reservation);
        void Remove(Reservation reservation);

        Task<IReadOnlyList<Reservation>> GetActiveFutureByRoomAsync(Guid roomId); 
        Task<IReadOnlyList<Reservation>> GetActiveFutureByRoomIdsAsync(IReadOnlyList<Guid> roomIds);
    }
}
