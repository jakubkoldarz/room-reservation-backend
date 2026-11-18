using RoomReservation.Core.Enums;
using RoomReservation.Core.Extensions;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Services
{
    public class ReservationExpiryService(
        IReservationRepository reservationRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider) : IReservationExpiryService
    {
        public async Task<int> ExpireStartedRequestsAsync()
        {
            var expiredCount = 0;

            await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var startedRequests = await reservationRepository.LockStartedPendingAsync(timeProvider.WarsawNow());
                foreach (var reservation in startedRequests)
                    reservation.Status = ReservationStatus.Expired;

                expiredCount = startedRequests.Count;
                return Result.Success();
            });

            return expiredCount;
        }
    }
}
