using RoomReservation.Api.Dtos.Reservations.Responses;
using RoomReservation.Core.Entities;

namespace RoomReservation.Api.Extensions.Mappers
{
    public static class ReservationMapperExtensions
    {
        public static ReservationResponse ToBasicDto(this Reservation reservation)
        {
            return new ReservationResponse(
                Id: reservation.Id,

                CreatedBy: reservation.CreatedBy.ToActorDto(reservation.CreatedAt)!,
                ApprovedBy: reservation.ApprovedBy.ToActorDto(reservation.ApprovedAt),
                CanceledBy: reservation.CanceledBy.ToActorDto(reservation.CanceledAt),
                RejectedBy: reservation.RejectedBy.ToActorDto(reservation.RejectedAt),

                Date: reservation.Date,
                StartTime: reservation.StartTime,
                EndTime: reservation.EndTime,
                Purpose: reservation.Purpose,
                Reason: reservation.Reason,
                Status: reservation.Status.ToString().ToUpper()
            );
        }

        private static ReservationActorResponse? ToActorDto(this User? user, DateTime? at)
        {
            if (user == null || at == null) return null;

            return new ReservationActorResponse(
                (DateTime)at,
                user.ToBasicDto()
            );
        }
    }
}
