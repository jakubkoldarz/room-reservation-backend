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
        public async Task<Result> ApproveAsync(Guid reservationId, Guid approvedById)
        {
            var reservationToUpdate = await reservationRepository.GetByIdAsync(reservationId);
            if (reservationToUpdate is null)
                return new Error("Reservation not found", ErrorType.NotFound);

            if (reservationToUpdate.Status is not ReservationStatus.Pending)
                return new Error("Reservation has invalid status", ErrorType.BadRequest);

            reservationToUpdate.ApprovedById = approvedById;
            reservationToUpdate.ApprovedAt = timeProvider.UtcNow();
            reservationToUpdate.Status = ReservationStatus.Approved;

            if (reservationToUpdate.CreatedBy is not null)
            {
                var approvedByName = await GetActorNameAsync(approvedById);
                EnqueueApproveNotification(reservationToUpdate.CreatedBy, approvedByName, reservationToUpdate);
            }

            await unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<Result> SelfCancelAsync(Guid reservationId, string? reason, Guid cancelledById)
        {
            var reservationToUpdate = await reservationRepository.GetByIdAsync(reservationId);
            if (reservationToUpdate is null || reservationToUpdate.CreatedById != cancelledById)
                return new Error("Reservation not found", ErrorType.NotFound);

            if (reservationToUpdate.Status is not (ReservationStatus.Approved))
                return new Error("Reservation has invalid status", ErrorType.BadRequest);

            reservationToUpdate.CanceledById = reservationToUpdate.CreatedById;
            reservationToUpdate.CanceledAt = timeProvider.UtcNow();
            reservationToUpdate.Reason = reason;
            reservationToUpdate.Status = ReservationStatus.Canceled;
            await unitOfWork.SaveChangesAsync();

            return Result.Success();
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
                if (HasOverlap(startTime, endTime, existingReservations))
                    return new Error("Reservation is in conflict with existing ones", ErrorType.Conflict);

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
            var reservationToDelete = await reservationRepository.GetByIdAsync(reservationId);
            if (reservationToDelete is null || reservationToDelete.CreatedById != requestingUserId)
                return new Error("Reservation not found", ErrorType.NotFound);

            if (reservationToDelete.Status is not ReservationStatus.Pending)
                return new Error("Only Pending reservations can be deleted", ErrorType.BadRequest);

            reservationRepository.Remove(reservationToDelete);
            await unitOfWork.SaveChangesAsync();
            return Result.Success();
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
            var reservationToUpdate = await reservationRepository.GetByIdAsync(reservationId);
            if (reservationToUpdate is null)
                return new Error("Reservation not found", ErrorType.NotFound);

            if (reservationToUpdate.Status is not ReservationStatus.Pending)
                return new Error("Reservation has invalid status", ErrorType.BadRequest);

            reservationToUpdate.RejectedById = rejectedById;
            reservationToUpdate.RejectedAt = timeProvider.UtcNow();
            reservationToUpdate.Reason = reason;
            reservationToUpdate.Status = ReservationStatus.Rejected;

            if (reservationToUpdate.CreatedBy is not null)
            {
                var rejectedByName = await GetActorNameAsync(rejectedById);
                EnqueueRejectNotification(reservationToUpdate.CreatedBy, rejectedByName, reservationToUpdate);
            }

            await unitOfWork.SaveChangesAsync();
            return Result.Success();
        }

        public async Task<ResultT<Reservation>> UpdateAsync(Guid requestingUserId, Guid reservationId, TimeOnly startTime, TimeOnly endTime, string? purpose = null)
        {
            var reservationToUpdate = await reservationRepository.GetByIdAsync(reservationId);
            if (reservationToUpdate is null || reservationToUpdate.CreatedById != requestingUserId)
                return new Error("Reservation not found", ErrorType.NotFound);

            if (reservationToUpdate.Status is not (ReservationStatus.Pending or ReservationStatus.Approved))
                return new Error("Reservation cannot be updated after being rejected or cancelled", ErrorType.BadRequest);

            if (startTime >= endTime)
                return new Error("Invalid timeframe provided", ErrorType.BadRequest);

            if (startTime != reservationToUpdate.StartTime && IsInThePast(reservationToUpdate.Date, startTime))
                return new Error("Reservation start is in the past", ErrorType.BadRequest);

            var availability = await availabilityService.ResolveAvailabilityAsync(reservationToUpdate.RoomId, reservationToUpdate.Date);
            if (!availability.Covers(startTime, endTime))
                return new Error("Selected room is not available at provided time", ErrorType.BadRequest);

            var result = await unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await reservationRepository.LockRoomAsync(reservationToUpdate.RoomId);

                var existingReservations = await reservationRepository.GetActiveByRoomAndDateAsync(reservationToUpdate.RoomId, reservationToUpdate.Date);
                if (HasOverlap(startTime, endTime, existingReservations, excludeReservationId: reservationId))
                    return new Error("Reservation is in conflict with existing ones", ErrorType.Conflict);

                var hasTimeframeChanged = startTime != reservationToUpdate.StartTime || endTime != reservationToUpdate.EndTime;
                if (hasTimeframeChanged && reservationToUpdate.Room.RequiresApproval)
                    reservationToUpdate.Status = ReservationStatus.Pending;

                reservationToUpdate.StartTime = startTime;
                reservationToUpdate.EndTime = endTime;
                reservationToUpdate.Purpose = purpose;
                return Result.Success();
            });

            if (!result.IsSuccess)
                return result.Error;

            return ResultT<Reservation>.Success(reservationToUpdate);
        }

        public async Task<Result> ForceCancelAsync(Guid reservationId, string? reason, Guid? cancelledById = null)
        {
            var reservationToUpdate = await reservationRepository.GetByIdAsync(reservationId);
            if (reservationToUpdate is null)
                return new Error("Reservation not found", ErrorType.NotFound);

            if (reservationToUpdate.Status is not (ReservationStatus.Pending or ReservationStatus.Approved))
                return new Error("Reservation has invalid status", ErrorType.BadRequest);

            var cancelledByName = await GetActorNameAsync(cancelledById);
            CancelWithNotification(reservationToUpdate, reason, cancelledById, cancelledByName);

            await unitOfWork.SaveChangesAsync();
            return Result.Success();
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
            var title = "Twoja rezerwacja została anulowana";

            var messageToSend = new ReservationCancelledEmail
            {
                To = recipient.Email,
                ActionUrl = "#",
                Title = title,
                RoomName = reservation.Room.Identifier,
                BuildingName = GetBuildingName(reservation.Room.Building.Name, reservation.Room.Building.Identifier),
                Date = reservation.Date,
                StartTime = reservation.StartTime,
                EndTime = reservation.EndTime,
                CancelReason = reservation.Reason ?? "Brak powodu podanego przez administratora",
                CancelledBy = cancelledByName,
                CancelledAt = reservation.CanceledAt!.Value
            };

            emailQueue.Enqueue(messageToSend);
        }

        private void EnqueueRejectNotification(User recipient, string rejectedByName, Reservation reservation)
        {
            var title = "Twoja prośba o rezerwacje została odrzucona";

            var messageToSend = new ReservationRejectedEmail
            {
                To = recipient.Email,
                ActionUrl = "#",
                Title = title,
                RoomName = reservation.Room.Identifier,
                BuildingName = GetBuildingName(reservation.Room.Building.Name, reservation.Room.Building.Identifier),
                Date = reservation.Date,
                StartTime = reservation.StartTime,
                EndTime = reservation.EndTime,
                RejectReason = reservation.Reason ?? "Brak powodu podanego przez administratora",
                RejectedBy = rejectedByName,
                RejectedAt = reservation.RejectedAt!.Value
            };

            emailQueue.Enqueue(messageToSend);
        }

        private void EnqueueApproveNotification(User recipient, string approvedByName, Reservation reservation)
        {
            var title = "Twoja rezerwacja została potwierdzona";

            var messageToSend = new ReservationApprovedEmail
            {
                To = recipient.Email,
                ActionUrl = "#",
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

        private static bool HasOverlap(
           TimeOnly start, TimeOnly end,
           IEnumerable<Reservation> existingReservations,
           Guid? excludeReservationId = null)
        {
            return existingReservations
                .Where(r => excludeReservationId is null || r.Id != excludeReservationId)
                .Any(r => start < r.EndTime && r.StartTime < end);
        }

        private bool IsInThePast(DateOnly date, TimeOnly startTime)
            => date.ToDateTime(startTime) < timeProvider.WarsawNow();
    }
}
