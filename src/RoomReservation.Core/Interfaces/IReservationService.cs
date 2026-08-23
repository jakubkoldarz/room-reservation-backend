using RoomReservation.Core.Entities;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Interfaces
{
    public interface IReservationService
    {
        Task<ResultT<Reservation>> GetByIdAsync(Guid reservationId);
        Task<PagedResult<Reservation>> GetAllAsync(ReservationFilter filters);
        Task<ResultT<Reservation>> CreateAsync(Guid roomId, DateOnly date, TimeOnly startTime, TimeOnly endTime, Guid createdById, string? purpose = null);
        Task<ResultT<Reservation>> UpdateAsync(Guid requestingUserId, Guid reservationId, TimeOnly startTime, TimeOnly endTime, string? purpose = null);
        Task<Result> SelfCancelAsync(Guid reservationId, string? reason, Guid cancelledById);
        Task<Result> ForceCancelAsync(Guid reservationId, string? reason, Guid cancelledById);
        Task<Result> RejectAsync(Guid reservationId, string? reason, Guid rejectedById);
        Task<Result> ApproveAsync(Guid reservationId, Guid approvedById);
        Task<Result> DeleteAsync(Guid reservationId, Guid requestingUserId);
    }
}
