using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;
using RoomReservation.Core.Providers;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Services
{
    public partial class ReservationService(
        IReservationRepository _reservations,
        IUserRepository _users,
        IBuildingRepository _buildings,
        IEmailService _emailService,
        IRoomRepository _rooms) : IReservationService
    {
        public async Task<Result> ApproveAsync(Guid reservationId, Guid approvedById)
        {
            var reservationToUpdate = await _reservations.GetByIdAsync(reservationId);
            if (reservationToUpdate == null)
                return new Error("Reservation not found", ErrorType.NotFound);

            if (reservationToUpdate.Status is not ReservationStatus.Pending)
                return new Error("Reservation has invalid status", ErrorType.BadRequest);

            reservationToUpdate.ApprovedById = approvedById;
            reservationToUpdate.ApprovedAt = DateTime.UtcNow;
            reservationToUpdate.Status = ReservationStatus.Approved;
            await _reservations.UpdateAsync(reservationToUpdate);

            if (reservationToUpdate.CreatedBy != null)
            {
                var approvedByUser = await _users.GetByIdAsync(approvedById);
                await SendApproveNotification(reservationToUpdate.CreatedBy, approvedByUser?.Firstname ?? "System", reservationToUpdate);
            }

            return Result.Success();
        }

        public async Task<Result> SelfCancelAsync(Guid reservationId, string? reason, Guid cancelledById)
        {
            var reservationToUpdate = await _reservations.GetByIdAsync(reservationId);
            if (reservationToUpdate == null || reservationToUpdate.CreatedById != cancelledById)
                return new Error("Reservation not found", ErrorType.NotFound);

            if (reservationToUpdate.Status is not (ReservationStatus.Approved))
                return new Error("Reservation has invalid status", ErrorType.BadRequest);

            reservationToUpdate.CanceledById = reservationToUpdate.CreatedById;
            reservationToUpdate.CanceledAt = DateTime.UtcNow;
            reservationToUpdate.Reason = reason;
            reservationToUpdate.Status = ReservationStatus.Canceled;
            await _reservations.UpdateAsync(reservationToUpdate);

            return Result.Success();
        }

        public async Task<ResultT<Reservation>> CreateAsync(Guid roomId, DateOnly date, TimeOnly startTime, TimeOnly endTime, Guid createdById, string? purpose = null)
        {
            if (startTime >= endTime)
                return new Error("Invalid timeframe provided", ErrorType.BadRequest);

            var room = await _rooms.GetByIdAsync(roomId);
            if (room == null)
                return new Error("Room was not found", ErrorType.NotFound);

            var existingReservations = await _reservations.GetActiveByRoomAndDateAsync(roomId, date);
            if (existingReservations.Count > 0)
            {
                var isOverlaping = AvailabilityProvider.HasOverlap(startTime, endTime, existingReservations);
                if (isOverlaping)
                    return new Error("Reseravtion is in conflict with existing ones", ErrorType.Conflict);
            }

            var availability = await ResolveAvailability(room.Id, room.BuildingId, date);
            var isWithinAvailability = AvailabilityProvider.IsWithinAvailability(startTime, endTime, availability);
            if (!isWithinAvailability)
                return new Error("Selected room is not available at provided time", ErrorType.BadRequest);

            var statusToSet = room.RequiresApproval ? ReservationStatus.Pending : ReservationStatus.Approved;

            var reservationToAdd = new Reservation
            {
                RoomId = roomId,
                CreatedById = createdById,
                Date = date,
                StartTime = startTime,
                EndTime = endTime,
                Purpose = purpose,
                Status = statusToSet
            };

            await _reservations.AddAsync(reservationToAdd);

            var createdReservation = await _reservations.GetByIdAsync(reservationToAdd.Id);
            if (createdReservation == null)
                return new Error("Reservation cannot be retrieved", ErrorType.Internal);
            return ResultT<Reservation>.Success(createdReservation);
        }

        public async Task<Result> DeleteAsync(Guid reservationId, Guid requestingUserId)
        {
            var reservationToDelete = await _reservations.GetByIdAsync(reservationId);
            if (reservationToDelete == null || reservationToDelete.CreatedById != requestingUserId)
                return new Error("Reservation not found", ErrorType.NotFound);

            if (reservationToDelete.Status is not ReservationStatus.Pending)
                return new Error("Only Pending reservations can be deleted", ErrorType.BadRequest);

            await _reservations.DeleteAsync(reservationToDelete);
            return Result.Success();
        }

        public async Task<PagedResult<Reservation>> GetAllAsync(ReservationFilter filters)
        {
            var (Reservations, TotalCount) = await _reservations.GetFilteredAsync(filters);
            return PagedResult<Reservation>.Success(
                Reservations,
                TotalCount,
                filters.Page,
                filters.PageSize);
        }

        public async Task<ResultT<Reservation>> GetByIdAsync(Guid reservationId)
        {
            var reservation = await _reservations.GetByIdAsync(reservationId);
            if (reservation == null)
                return new Error("Reservation not found", ErrorType.NotFound);

            return ResultT<Reservation>.Success(reservation);
        }

        public async Task<Result> RejectAsync(Guid reservationId, string? reason, Guid rejectedById)
        {
            var reservationToUpdate = await _reservations.GetByIdAsync(reservationId);
            if (reservationToUpdate == null)
                return new Error("Reservation not found", ErrorType.NotFound);

            if (reservationToUpdate.Status is not ReservationStatus.Pending)
                return new Error("Reseravtion has invalid status", ErrorType.BadRequest);

            reservationToUpdate.RejectedById = rejectedById;
            reservationToUpdate.RejectedAt = DateTime.UtcNow;
            reservationToUpdate.Reason = reason;
            reservationToUpdate.Status = ReservationStatus.Rejected;
            await _reservations.UpdateAsync(reservationToUpdate);

            if (reservationToUpdate.CreatedBy != null)
            {
                var rejectingUser = await _users.GetByIdAsync(rejectedById);
                await SendRejectNotification(reservationToUpdate.CreatedBy, rejectingUser?.Firstname ?? "System", reservationToUpdate);
            }

            return Result.Success();
        }

        public async Task<ResultT<Reservation>> UpdateAsync(Guid requestingUserId, Guid reservationId, TimeOnly startTime, TimeOnly endTime, string? purpose = null)
        {
            var reservationToUpdate = await _reservations.GetByIdAsync(reservationId);
            if (reservationToUpdate == null || reservationToUpdate.CreatedById != requestingUserId)
                return new Error("Reservation not found", ErrorType.NotFound);

            if (reservationToUpdate.Status is not (ReservationStatus.Pending or ReservationStatus.Approved))
                return new Error("Reseravtion cannot be updated after being rejected or cancelled", ErrorType.BadRequest);

            if (startTime >= endTime)
                return new Error("Invalid timeframe provided", ErrorType.BadRequest);

            var existingReservations = await _reservations.GetActiveByRoomAndDateAsync(reservationToUpdate.RoomId, reservationToUpdate.Date);
            if (existingReservations.Count > 0)
            {
                var isOverlaping = AvailabilityProvider.HasOverlap(startTime, endTime, existingReservations, excludeReservationId: reservationId);
                if (isOverlaping)
                    return new Error("Reseravtion is in conflict with existing ones", ErrorType.Conflict);
            }

            var availability = await ResolveAvailability(reservationToUpdate.RoomId, reservationToUpdate.Room.BuildingId, reservationToUpdate.Date);
            var isWithinAvailability = AvailabilityProvider.IsWithinAvailability(startTime, endTime, availability);
            if (!isWithinAvailability)
                return new Error("Selected room is not available at provided time", ErrorType.BadRequest);

            var hasTimeframeChanged = startTime != reservationToUpdate.StartTime || endTime != reservationToUpdate.EndTime;
            if (hasTimeframeChanged && reservationToUpdate.Room.RequiresApproval)
            {
                reservationToUpdate.Status = ReservationStatus.Pending;
            }

            reservationToUpdate.StartTime = startTime;
            reservationToUpdate.EndTime = endTime;
            reservationToUpdate.Purpose = purpose;

            await _reservations.UpdateAsync(reservationToUpdate);

            var updatedReservation = await _reservations.GetByIdAsync(reservationId);
            if (updatedReservation == null)
                return new Error("Reservation cannot be retrieved", ErrorType.Internal);
            return ResultT<Reservation>.Success(updatedReservation);
        }

        private async Task<AvailabilityResolution> ResolveAvailability(Guid roomId, Guid buildingId, DateOnly date)
        {
            var roomSpecial = await _rooms.GetSpecialAvailabilityByDateAsync(roomId, date);
            if (roomSpecial != null)
            {
                if (roomSpecial.IsClosed) return new AvailabilityResolution(true, null, null);
                return new AvailabilityResolution(false, roomSpecial.StartTime, roomSpecial.EndTime);
            }

            var buildingSpecial = await _buildings.GetSpecialAvailabilityByDateAsync(buildingId, date);
            if (buildingSpecial != null)
            {
                if (buildingSpecial.IsClosed) return new AvailabilityResolution(true, null, null);
                return new AvailabilityResolution(false, buildingSpecial.StartTime, buildingSpecial.EndTime);
            }

            var roomAvailability = await _rooms.GetAvailabilityByDateAsync(roomId, date);
            if (roomAvailability != null)
            {
                return new AvailabilityResolution(false, roomAvailability.StartTime, roomAvailability.EndTime);
            }

            var buildingAvailability = await _buildings.GetAvailabilityByDateAsync(buildingId, date);
            if (buildingAvailability != null)
            {
                return new AvailabilityResolution(false, buildingAvailability.StartTime, buildingAvailability.EndTime);
            }

            return new AvailabilityResolution(true, null, null);
        }

        public async Task<Result> ForceCancelAsync(Guid reservationId, string? reason, Guid cancelledById)
        {
            var reservationToUpdate = await _reservations.GetByIdAsync(reservationId);
            if (reservationToUpdate == null)
                return new Error("Reservation not found", ErrorType.NotFound);

            if (reservationToUpdate.Status is not (ReservationStatus.Pending or ReservationStatus.Approved))
                return new Error("Reservation has invalid status", ErrorType.BadRequest);

            reservationToUpdate.CanceledById = cancelledById;
            reservationToUpdate.CanceledAt = DateTime.UtcNow;
            reservationToUpdate.Reason = reason;
            reservationToUpdate.Status = ReservationStatus.Canceled;
            await _reservations.UpdateAsync(reservationToUpdate);

            if (reservationToUpdate.CreatedBy != null)
            {
                var cancellingUser = await _users.GetByIdAsync(cancelledById);
                await SendCancelNotification(reservationToUpdate.CreatedBy, cancellingUser?.Firstname ?? "System", reservationToUpdate);
            }

            return Result.Success();
        }

        private async Task<Result> SendCancelNotification(User recipient, string cancelledByName, Reservation reservation)
        {
            var subject = "Rezerwacja anulowana";
            var title = "Twoja rezerwacja została anulowana";

            var messageResult = await _emailService.GetMessageAsync("CancelReservation", new Dictionary<string, string>
            {
                ["Title"] = title,
                ["RoomName"] = reservation.Room.Identifier,
                ["BuildingName"] = GetBuildingName(reservation.Room.Building.Name, reservation.Room.Building.Identifier),
                ["Date"] = reservation.Date.ToString("dd-MM-yyyy"),
                ["StartTime"] = reservation.StartTime.ToString("HH:mm"),
                ["EndTime"] = reservation.EndTime.ToString("HH:mm"),
                ["CancelReason"] = reservation.Reason ?? "Brak powodu podanego przez administratora",
                ["CanceledBy"] = cancelledByName,
                ["CancelledAt"] = reservation.CanceledAt!.Value.ToString("dd-MM-yyyy HH:mm"),
            });

            if (!messageResult.IsSuccess)
                return messageResult.Error;

            return await _emailService.SendEmailAsync(new EmailMessage
            {
                To = recipient.Email,
                Subject = subject,
                HtmlMessage = messageResult.Value,
            });
        }

        private async Task<Result> SendRejectNotification(User recipient, string rejectedByName, Reservation reservation)
        {
            var subject = "Rezerwacja odrzucona";
            var title = "Twoja prośba o rezerwacje została odrzucona";

            var messageResult = await _emailService.GetMessageAsync("RejectReservation", new Dictionary<string, string>
            {
                ["Title"] = title,
                ["RoomName"] = reservation.Room.Identifier,
                ["BuildingName"] = GetBuildingName(reservation.Room.Building.Name, reservation.Room.Building.Identifier),
                ["Date"] = reservation.Date.ToString("dd-MM-yyyy"),
                ["StartTime"] = reservation.StartTime.ToString("HH:mm"),
                ["EndTime"] = reservation.EndTime.ToString("HH:mm"),
                ["RejectReason"] = reservation.Reason ?? "Brak powodu podanego przez administratora",
                ["RejectedBy"] = rejectedByName,
                ["RejectedAt"] = reservation.RejectedAt!.Value.ToString("dd-MM-yyyy HH:mm"),
            });

            if (!messageResult.IsSuccess)
                return messageResult.Error;

            return await _emailService.SendEmailAsync(new EmailMessage
            {
                To = recipient.Email,
                Subject = subject,
                HtmlMessage = messageResult.Value,
            });
        }

        private async Task<Result> SendApproveNotification(User recipient, string approvedByName, Reservation reservation)
        {
            var subject = "Rezerwacja zaakceptowana";
            var title = "Twoja rezerwacja została potwierdzona";

            var messageResult = await _emailService.GetMessageAsync("ApproveReservation", new Dictionary<string, string>
            {
                ["Title"] = title,
                ["RoomName"] = reservation.Room.Identifier,
                ["BuildingName"] = GetBuildingName(reservation.Room.Building.Name, reservation.Room.Building.Identifier),
                ["Date"] = reservation.Date.ToString("dd-MM-yyyy"),
                ["StartTime"] = reservation.StartTime.ToString("HH:mm"),
                ["EndTime"] = reservation.EndTime.ToString("HH:mm"),
                ["ApprovedBy"] = approvedByName,
                ["ApprovedAt"] = reservation.ApprovedAt!.Value.ToString("dd-MM-yyyy HH:mm"),
            });

            if (!messageResult.IsSuccess)
                return messageResult.Error;

            return await _emailService.SendEmailAsync(new EmailMessage
            {
                To = recipient.Email,
                Subject = subject,
                HtmlMessage = messageResult.Value,
            });
        }

        private static string GetBuildingName(string buildingName, string? buildingIdentifier) => $"{buildingName}" + $"{(buildingIdentifier != null ? $" ({buildingIdentifier})" : "")}";
    }
}
