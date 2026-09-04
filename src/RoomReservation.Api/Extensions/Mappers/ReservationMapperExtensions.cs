using RoomReservation.Api.Dtos.Reservations.Responses;
using RoomReservation.Core.Entities;

namespace RoomReservation.Api.Extensions.Mappers
{
    public static class ReservationMapperExtensions
    {
        public static ReservationResponseDto ToBasicDto(this Reservation reservation)
        {
            return new ReservationResponseDto(
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

        private static ReservationActorResponseDto? ToActorDto(this User? user, DateTime? at)
        {
            if (user == null || at == null) return null;

            return new ReservationActorResponseDto(
                (DateTime)at,
                user.ToBasicDto()
            );
        }
    }
}
