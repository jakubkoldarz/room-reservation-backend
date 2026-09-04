using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;

namespace RoomReservation.Core.Interfaces
{
    public interface IReservationRepository
    {
        Task<Reservation?> GetByIdAsync(Guid reservationId);
        Task<(IReadOnlyList<Reservation> Reservations, int TotalCount)> GetFilteredAsync(ReservationFilter filters);
        Task<IReadOnlyList<Reservation>> GetActiveByRoomAndDateAsync(Guid roomId, DateOnly date);
        Task AddAsync(Reservation reservation);
        Task UpdateAsync(Reservation reservation);
        Task DeleteAsync(Reservation reservation);

        Task<IReadOnlyList<Reservation>> GetActiveFutureByRoomAsync(Guid roomId); 
        Task<IReadOnlyList<Reservation>> GetActiveFutureByRoomIdsAsync(IReadOnlyList<Guid> roomIds);
    }
}
