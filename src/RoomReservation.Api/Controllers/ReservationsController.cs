using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RoomReservation.Api.Attributes;
using RoomReservation.Api.Dtos;
using RoomReservation.Api.Dtos.Reservations.Requests;
using RoomReservation.Api.Dtos.Reservations.Responses;
using RoomReservation.Api.Extensions;
using RoomReservation.Api.Extensions.Mappers;
using RoomReservation.Core.Constants;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;

namespace RoomReservation.Api.Controllers
{
    [EnableRateLimiting("default")]
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ReservationsController(IReservationService reservationService) : ControllerBase
    {
        [HttpGet("{reservationId}")]
        [RequirePermission(Permissions.ReservationView)]
        public async Task<ActionResult<ReservationResponseDto>> GetSingle(Guid reservationId)
        {
            var result = await reservationService.GetByIdAsync(reservationId);
            return result.ToActionResult(reservation => Ok(reservation.ToBasicDto()));
        }

        [HttpGet]
        [RequirePermission(Permissions.ReservationList)]
        public async Task<ActionResult<PagedResponseDto<ReservationResponseDto>>> GetAll([FromQuery] ReservationFilter filters)
        {
            var result = await reservationService.GetAllAsync(filters);
            return result.ToActionResult(page => Ok(page.ToPagedDto(r => r.ToBasicDto())));
        }

        [HttpGet("mine")]
        [RequireCompletedProfile]
        public async Task<ActionResult<PagedResponseDto<ReservationResponseDto>>> GetMyReservations([FromQuery] ReservationFilter filters, [UserId] Guid userId)
        {
            filters.CreatedById = userId;
            var result = await reservationService.GetAllAsync(filters);
            return result.ToActionResult(page => Ok(page.ToPagedDto(r => r.ToBasicDto())));
        }

        [HttpPost]
        [RequirePermission(Permissions.ReservationCreate)]
        public async Task<ActionResult<ReservationResponseDto>> Create([FromBody] CreateReservationRequestDto request, [UserId] Guid userId)
        {
            var result = await reservationService.CreateAsync(
                createdById: userId,
                roomId: request.RoomId,
                date: request.Date,
                startTime: request.StartTime,
                endTime: request.EndTime,
                purpose: request.Purpose
            );
            return result.ToActionResult(reservation =>
                CreatedAtAction(nameof(GetSingle), new { reservationId = reservation.Id }, reservation.ToBasicDto()));
        }

        [HttpPost("{reservationId:guid}/approve")]
        [RequirePermission(Permissions.ReservationApprove)]
        public async Task<IActionResult> Approve(Guid reservationId, [UserId] Guid userId)
        {
            var result = await reservationService.ApproveAsync(reservationId, approvedById: userId);
            return result.ToActionResult(NoContent);
        }

        [HttpPost("{reservationId:guid}/reject")]
        [RequirePermission(Permissions.ReservationReject)]
        public async Task<IActionResult> Reject(Guid reservationId, [FromBody] ReservationReasonRequestDto request, [UserId] Guid userId)
        {
            var result = await reservationService.RejectAsync(reservationId, rejectedById: userId, reason: request.Reason);
            return result.ToActionResult(NoContent);
        }

        [HttpPost("{reservationId:guid}/cancel")]
        [RequireCompletedProfile]
        public async Task<IActionResult> Cancel(Guid reservationId, [FromBody] ReservationReasonRequestDto request, [UserId] Guid userId)
        {
            var result = await reservationService.SelfCancelAsync(reservationId, reason: request.Reason, cancelledById: userId);
            return result.ToActionResult(NoContent);
        }

        [HttpPost("{reservationId:guid}/force-cancel")]
        [RequirePermission(Permissions.ReservationForceCancel)]
        public async Task<IActionResult> ForceCancel(Guid reservationId, [FromBody] ReservationReasonRequestDto request, [UserId] Guid userId)
        {
            var result = await reservationService.ForceCancelAsync(reservationId, reason: request.Reason, cancelledById: userId);
            return result.ToActionResult(NoContent);
        }

        [HttpDelete("{reservationId:guid}")]
        [RequireCompletedProfile]
        public async Task<IActionResult> Delete(Guid reservationId, [UserId] Guid userId)
        {
            var result = await reservationService.DeleteAsync(reservationId, requestingUserId: userId);
            return result.ToActionResult(NoContent);
        }

        [HttpPut("{reservationId:guid}")]
        [RequireCompletedProfile]
        public async Task<ActionResult<ReservationResponseDto>> Update(Guid reservationId, [FromBody] UpdateReservationRequestDto request, [UserId] Guid userId)
        {
            var result = await reservationService.UpdateAsync(
                requestingUserId: userId,
                reservationId: reservationId,
                startTime: request.StartTime,
                endTime: request.EndTime,
                purpose: request.Purpose
            );
            return result.ToActionResult(reservation => Ok(reservation.ToBasicDto()));
        }
    }
}
