using RoomReservation.Core.Emails;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Extensions;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Services
{
    public class ReservationService(
        IReservationRepository reservationRepository,
        IUserRepository userRepository,
        IAvailabilityService availabilityService,
        IEmailQueue emailQueue,
        IRoomRepository roomRepository,
        IUnitOfWork unitOfWork,
        TimeProvider timeProvider) : IReservationService
    {
        private const string AlreadyStartedMessage = "Reservation has already started";
        private const string NotFoundMessage = "Reservation not found";
        private const string InvalidStatusMessage = "Reservation has invalid status";
        private const string OverlapMessage = "Reservation is in conflict with existing ones";
        private const string ReplacedByApprovalReason = "Another request for this time was approved";

        public async Task<Result> ApproveAsync(Guid reservationId, Guid approvedById)
        {
            return await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var roomId = await reservationRepository.GetRoomIdAsync(reservationId);
                if (roomId is null)
                    return new Error(NotFoundMessage, ErrorType.NotFound);

                await reservationRepository.LockRoomAsync(roomId.Value);

                var reservationToApprove = await LockAndGetAsync(reservationId);
                if (reservationToApprove is null)
                    return new Error(NotFoundMessage, ErrorType.NotFound);

                if (reservationToApprove.Status is not ReservationStatus.Pending)
                    return new Error(InvalidStatusMessage, ErrorType.BadRequest);

                if (HasStarted(reservationToApprove))
                    return new Error(AlreadyStartedMessage, ErrorType.BadRequest);

                await reservationRepository.LockPendingByRoomAndDateAsync(reservationToApprove.RoomId, reservationToApprove.Date);
                var dayReservations = await reservationRepository.GetActiveByRoomAndDateAsync(reservationToApprove.RoomId, reservationToApprove.Date);
                var overlapping = OverlappingReservations(reservationToApprove.StartTime, reservationToApprove.EndTime, dayReservations, reservationId);

                foreach (var other in overlapping)
                {
                    if (other.Status == ReservationStatus.Approved)
                        return new Error("Room is already booked at this time", ErrorType.Conflict);
                }

                var approvedByName = await GetActorNameAsync(approvedById);

                reservationToApprove.ApprovedById = approvedById;
                reservationToApprove.ApprovedAt = timeProvider.UtcNow();
                reservationToApprove.Status = ReservationStatus.Approved;

                if (reservationToApprove.CreatedBy is not null)
                    EnqueueApproveNotification(reservationToApprove.CreatedBy, approvedByName, reservationToApprove);

                foreach (var otherRequest in overlapping)
                    RejectWithNotification(otherRequest, ReplacedByApprovalReason, approvedById, approvedByName);

                return Result.Success();
            });
        }

        public async Task<Result> SelfCancelAsync(Guid reservationId, string? reason, Guid cancelledById)
        {
            return await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var reservationToCancel = await LockAndGetAsync(reservationId);
                if (reservationToCancel is null || reservationToCancel.CreatedById != cancelledById)
                    return new Error(NotFoundMessage, ErrorType.NotFound);

                if (reservationToCancel.Status is not ReservationStatus.Approved)
                    return new Error(InvalidStatusMessage, ErrorType.BadRequest);

                if (HasStarted(reservationToCancel))
                    return new Error(AlreadyStartedMessage, ErrorType.BadRequest);

                reservationToCancel.CanceledById = reservationToCancel.CreatedById;
                reservationToCancel.CanceledAt = timeProvider.UtcNow();
                reservationToCancel.Reason = reason;
                reservationToCancel.Status = ReservationStatus.Canceled;
                return Result.Success();
            });
        }

        public async Task<ResultT<Reservation>> CreateAsync(Guid roomId, DateOnly date, TimeOnly startTime, TimeOnly endTime, Guid createdById, string? purpose = null)
        {
            if (startTime >= endTime)
                return new Error("Invalid timeframe provided", ErrorType.BadRequest);

            if (IsInThePast(date, startTime))
                return new Error("Reservation start is in the past", ErrorType.BadRequest);

            var room = await roomRepository.GetByIdAsync(roomId);
            if (room is null)
                return new Error("Room was not found", ErrorType.NotFound);

            var availability = await availabilityService.ResolveAvailabilityAsync(roomId, date);
            if (!availability.Covers(startTime, endTime))
                return new Error("Selected room is not available at provided time", ErrorType.BadRequest);

            var reservationToAdd = new Reservation
            {
                RoomId = roomId,
                CreatedById = createdById,
                Date = date,
                StartTime = startTime,
                EndTime = endTime,
                Purpose = purpose,
                CreatedAt = timeProvider.UtcNow(),
                Status = room.RequiresApproval ? ReservationStatus.Pending : ReservationStatus.Approved
            };

            var result = await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await reservationRepository.LockRoomAsync(roomId);

                var existingReservations = await reservationRepository.GetActiveByRoomAndDateAsync(roomId, date);
                if (HasBlockingOverlap(room.RequiresApproval, createdById, startTime, endTime, existingReservations, excludeReservationId: null))
                    return new Error(OverlapMessage, ErrorType.Conflict);

                reservationRepository.Add(reservationToAdd);
                return Result.Success();
            });

            if (!result.IsSuccess)
                return result.Error;

            var createdReservation = await reservationRepository.GetByIdAsync(reservationToAdd.Id);
            if (createdReservation is null)
                return new Error("Reservation cannot be retrieved", ErrorType.Internal);
            return ResultT<Reservation>.Success(createdReservation);
        }

        public async Task<Result> DeleteAsync(Guid reservationId, Guid requestingUserId)
        {
            return await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var reservationToDelete = await LockAndGetAsync(reservationId);
                if (reservationToDelete is null || reservationToDelete.CreatedById != requestingUserId)
                    return new Error(NotFoundMessage, ErrorType.NotFound);

                if (reservationToDelete.Status is not ReservationStatus.Pending)
                    return new Error("Only Pending reservations can be deleted", ErrorType.BadRequest);

                if (HasStarted(reservationToDelete))
                    return new Error(AlreadyStartedMessage, ErrorType.BadRequest);

                reservationRepository.Remove(reservationToDelete);
                return Result.Success();
            });
        }

        public async Task<ResultT<PagedList<Reservation>>> GetAllAsync(ReservationFilter filters)
        {
            var reservations = await reservationRepository.GetFilteredAsync(filters);
            return ResultT<PagedList<Reservation>>.Success(reservations);
        }

        public async Task<ResultT<Reservation>> GetByIdAsync(Guid reservationId)
        {
            var reservation = await reservationRepository.GetByIdAsync(reservationId);
            if (reservation is null)
                return new Error("Reservation not found", ErrorType.NotFound);

            return ResultT<Reservation>.Success(reservation);
        }

        public async Task<Result> RejectAsync(Guid reservationId, string? reason, Guid rejectedById)
        {
            return await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var reservationToReject = await LockAndGetAsync(reservationId);
                if (reservationToReject is null)
                    return new Error(NotFoundMessage, ErrorType.NotFound);

                if (reservationToReject.Status is not ReservationStatus.Pending)
                    return new Error(InvalidStatusMessage, ErrorType.BadRequest);

                if (HasStarted(reservationToReject))
                    return new Error(AlreadyStartedMessage, ErrorType.BadRequest);

                var rejectedByName = await GetActorNameAsync(rejectedById);
                RejectWithNotification(reservationToReject, reason, rejectedById, rejectedByName);
                return Result.Success();
            });
        }

        public async Task<ResultT<Reservation>> UpdateAsync(Guid requestingUserId, Guid reservationId, TimeOnly startTime, TimeOnly endTime, string? purpose = null)
        {
            return await unitOfWork.ExecuteInTransactionAsync<Reservation>(async () =>
            {
                var roomId = await reservationRepository.GetRoomIdAsync(reservationId);
                if (roomId is null)
                    return new Error(NotFoundMessage, ErrorType.NotFound);

                await reservationRepository.LockRoomAsync(roomId.Value);

                var reservationToUpdate = await LockAndGetAsync(reservationId);
                if (reservationToUpdate is null || reservationToUpdate.CreatedById != requestingUserId)
                    return new Error(NotFoundMessage, ErrorType.NotFound);

                if (reservationToUpdate.Status is not (ReservationStatus.Pending or ReservationStatus.Approved))
                    return new Error("Reservation cannot be updated after being rejected or cancelled", ErrorType.BadRequest);

                if (HasStarted(reservationToUpdate))
                    return new Error(AlreadyStartedMessage, ErrorType.BadRequest);

                if (startTime >= endTime)
                    return new Error("Invalid timeframe provided", ErrorType.BadRequest);

                if (startTime != reservationToUpdate.StartTime && IsInThePast(reservationToUpdate.Date, startTime))
                    return new Error("Reservation start is in the past", ErrorType.BadRequest);

                var availability = await availabilityService.ResolveAvailabilityAsync(reservationToUpdate.RoomId, reservationToUpdate.Date);
                if (!availability.Covers(startTime, endTime))
                    return new Error("Selected room is not available at provided time", ErrorType.BadRequest);

                var existingReservations = await reservationRepository.GetActiveByRoomAndDateAsync(reservationToUpdate.RoomId, reservationToUpdate.Date);
                var requiresApproval = reservationToUpdate.Room.RequiresApproval;
                if (HasBlockingOverlap(requiresApproval, requestingUserId, startTime, endTime, existingReservations, reservationId))
                    return new Error(OverlapMessage, ErrorType.Conflict);

                var hasTimeframeChanged = startTime != reservationToUpdate.StartTime || endTime != reservationToUpdate.EndTime;
                if (hasTimeframeChanged && requiresApproval)
                    reservationToUpdate.Status = ReservationStatus.Pending;

                reservationToUpdate.StartTime = startTime;
                reservationToUpdate.EndTime = endTime;
                reservationToUpdate.Purpose = purpose;
                return ResultT<Reservation>.Success(reservationToUpdate);
            });
        }

        public async Task<Result> ForceCancelAsync(Guid reservationId, string? reason, Guid? cancelledById = null)
        {
            return await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var reservationToCancel = await LockAndGetAsync(reservationId);
                if (reservationToCancel is null)
                    return new Error(NotFoundMessage, ErrorType.NotFound);

                if (reservationToCancel.Status is not (ReservationStatus.Pending or ReservationStatus.Approved))
                    return new Error(InvalidStatusMessage, ErrorType.BadRequest);

                if (HasStarted(reservationToCancel))
                    return new Error(AlreadyStartedMessage, ErrorType.BadRequest);

                var cancelledByName = await GetActorNameAsync(cancelledById);
                CancelWithNotification(reservationToCancel, reason, cancelledById, cancelledByName);
                return Result.Success();
            });
        }

        public async Task<Result> BulkForceCancelAsync(IReadOnlyList<Reservation> reservations, string? reason, Guid? cancelledById = null)
        {
            if (reservations.Count == 0)
                return Result.Success();

            var reservationsToCancel = await reservationRepository.GetByIdsAsync([.. reservations.Select(r => r.Id)]);
            var cancelledByName = await GetActorNameAsync(cancelledById);

            foreach (var reservation in reservationsToCancel)
            {
                if (reservation.Status is ReservationStatus.Pending or ReservationStatus.Approved)
                    CancelWithNotification(reservation, reason, cancelledById, cancelledByName);
            }

            return Result.Success();
        }

        public async Task<IReadOnlyList<Reservation>> GetConflictingWithEventAsync(Event ev)
        {
            var activeReservations = await reservationRepository.GetActiveFutureByRoomIdsAsync([.. ev.Rooms.Select(rm => rm.Id)]);
            var relevantReservations = activeReservations.Where(r => ev.StartDate <= r.Date && r.Date <= ev.EndDate);

            if (ev.IsClosed)
                return [.. relevantReservations];

            var conflictingReservations = relevantReservations
                .Where(r => !(ev.StartTime <= r.StartTime && r.EndTime <= ev.EndTime));

            return [.. conflictingReservations];
        }

        private void CancelWithNotification(Reservation reservation, string? reason, Guid? cancelledById, string cancelledByName)
        {
            reservation.CanceledById = cancelledById;
            reservation.CanceledAt = timeProvider.UtcNow();
            reservation.Reason = reason;
            reservation.Status = ReservationStatus.Canceled;

            if (reservation.CreatedBy is not null)
                EnqueueCancelNotification(reservation.CreatedBy, cancelledByName, reservation);
        }

        private async Task<string> GetActorNameAsync(Guid? userId)
        {
            if (!userId.HasValue)
                return "System";

            var user = await userRepository.GetByIdAsync(userId.Value);
            return user?.Firstname ?? "System";
        }

        private void EnqueueCancelNotification(User recipient, string cancelledByName, Reservation reservation)
        {
            var title = "Your reservation was canceled";

            var messageToSend = new ReservationCancelledEmail
            {
                To = recipient.Email,
                Title = title,
                RoomName = reservation.Room.Identifier,
                BuildingName = GetBuildingName(reservation.Room.Building.Name, reservation.Room.Building.Identifier),
                Date = reservation.Date,
                StartTime = reservation.StartTime,
                EndTime = reservation.EndTime,
                CancelReason = reservation.Reason ?? "No reason given",
                CancelledBy = cancelledByName,
                CancelledAt = reservation.CanceledAt!.Value
            };

            emailQueue.Enqueue(messageToSend);
        }

        private void EnqueueRejectNotification(User recipient, string rejectedByName, Reservation reservation)
        {
            var title = "Your reservation request was rejected";

            var messageToSend = new ReservationRejectedEmail
            {
                To = recipient.Email,
                Title = title,
                RoomName = reservation.Room.Identifier,
                BuildingName = GetBuildingName(reservation.Room.Building.Name, reservation.Room.Building.Identifier),
                Date = reservation.Date,
                StartTime = reservation.StartTime,
                EndTime = reservation.EndTime,
                RejectReason = reservation.Reason ?? "No reason given",
                RejectedBy = rejectedByName,
                RejectedAt = reservation.RejectedAt!.Value
            };

            emailQueue.Enqueue(messageToSend);
        }

        private void EnqueueApproveNotification(User recipient, string approvedByName, Reservation reservation)
        {
            var title = "Your reservation was confirmed";

            var messageToSend = new ReservationApprovedEmail
            {
                To = recipient.Email,
                Title = title,
                RoomName = reservation.Room.Identifier,
                BuildingName = GetBuildingName(reservation.Room.Building.Name, reservation.Room.Building.Identifier),
                Date = reservation.Date,
                StartTime = reservation.StartTime,
                EndTime = reservation.EndTime,
                ApprovedAt = reservation.ApprovedAt!.Value,
                ApprovedBy = approvedByName
            };

            emailQueue.Enqueue(messageToSend);
        }

        private static string GetBuildingName(string buildingName, string? buildingIdentifier) => $"{buildingName}" + $"{(buildingIdentifier is not null ? $" ({buildingIdentifier})" : "")}";

        private async Task<Reservation?> LockAndGetAsync(Guid reservationId)
        {
            await reservationRepository.LockReservationAsync(reservationId);
            return await reservationRepository.GetByIdAsync(reservationId);
        }

        private void RejectWithNotification(Reservation reservation, string? reason, Guid rejectedById, string rejectedByName)
        {
            reservation.RejectedById = rejectedById;
            reservation.RejectedAt = timeProvider.UtcNow();
            reservation.Reason = reason;
            reservation.Status = ReservationStatus.Rejected;

            if (reservation.CreatedBy is not null)
                EnqueueRejectNotification(reservation.CreatedBy, rejectedByName, reservation);
        }

        private static List<Reservation> OverlappingReservations(
            TimeOnly start,
            TimeOnly end,
            IEnumerable<Reservation> dayReservations,
            Guid? excludeReservationId)
        {
            var overlapping = new List<Reservation>();
            foreach (var reservation in dayReservations)
            {
                if (reservation.Id == excludeReservationId)
                    continue;

                if (start < reservation.EndTime && reservation.StartTime < end)
                    overlapping.Add(reservation);
            }
            return overlapping;
        }

        private static bool HasBlockingOverlap(
            bool requiresApproval,
            Guid userId,
            TimeOnly start,
            TimeOnly end,
            IEnumerable<Reservation> dayReservations,
            Guid? excludeReservationId)
        {
            var overlapping = OverlappingReservations(start, end, dayReservations, excludeReservationId);
            foreach (var reservation in overlapping)
            {
                if (BlocksSameTime(reservation, requiresApproval, userId))
                    return true;
            }
            return false;
        }

        private static bool BlocksSameTime(Reservation existing, bool requiresApproval, Guid userId)
        {
            if (!requiresApproval)
                return true;

            if (existing.Status == ReservationStatus.Approved)
                return true;

            return existing.CreatedById == userId;
        }

        private bool IsInThePast(DateOnly date, TimeOnly startTime)
            => date.ToDateTime(startTime) < timeProvider.WarsawNow();

        private bool HasStarted(Reservation reservation)
            => IsInThePast(reservation.Date, reservation.StartTime);
    }
}
