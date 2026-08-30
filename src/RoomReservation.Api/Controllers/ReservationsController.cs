using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomReservation.Api.Attributes;
using RoomReservation.Api.Dtos.Reservations.Requests;
using RoomReservation.Api.Dtos.Reservations.Responses;
using RoomReservation.Api.Extensions;
using RoomReservation.Api.Extensions.Mappers;
using RoomReservation.Core.Constants;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class ReservationsController(IReservationService _reservationService) : ControllerBase
    {
        [HttpGet("{reservationId}")]
        [RequirePermission(Permissions.ReservationView)]
        public async Task<ActionResult<ReservationResponseDto>> GetSingle(Guid reservationId)
        {
            var result = await _reservationService.GetByIdAsync(reservationId);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return Ok(result.Value.ToBasicDto());
        }

        [HttpGet]
        [RequirePermission(Permissions.ReservationList)]
        public async Task<ActionResult<PagedResult<ReservationResponseDto>>> GetAll([FromQuery] ReservationFilter filters)
        {
            var result = await _reservationService.GetAllAsync(filters);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return Ok(result.ToDto(r => r.ToBasicDto()));
        }

        [HttpGet("mine")]
        [RequireCompletedProfile]
        public async Task<ActionResult<PagedResult<ReservationResponseDto>>> GetMyReservations([FromQuery] ReservationFilter filters)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            filters.CreatedById = userId;
            var result = await _reservationService.GetAllAsync(filters);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return Ok(result.ToDto(r => r.ToBasicDto()));
        }

        [HttpPost]
        [RequirePermission(Permissions.ReservationCreate)]
        public async Task<ActionResult<ReservationResponseDto>> Create([FromBody] CreateReservationRequestDto request)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var result = await _reservationService.CreateAsync(
                createdById: (Guid)userId,
                roomId: request.RoomId,
                date: request.Date,
                startTime: request.StartTime,
                endTime: request.EndTime,
                purpose: request.Purpose
            );
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return CreatedAtAction(nameof(GetSingle), new { reservationId = result.Value.Id }, result.Value.ToBasicDto());
        }

        [HttpPost("{reservationId:guid}/approve")]
        [RequirePermission(Permissions.ReservationApprove)]
        public async Task<ActionResult> Approve(Guid reservationId)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var result = await _reservationService.ApproveAsync(reservationId, approvedById: (Guid)userId);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return NoContent();
        }


        [HttpPost("{reservationId:guid}/reject")]
        [RequirePermission(Permissions.ReservationReject)]
        public async Task<ActionResult> Reject(Guid reservationId, [FromBody] ReservationReasonRequestDto request)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var result = await _reservationService.RejectAsync(reservationId, rejectedById: (Guid)userId, reason: request.Reason);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return NoContent();
        }

        [HttpPost("{reservationId:guid}/cancel")]
        [RequireCompletedProfile]
        public async Task<ActionResult> Cancel(Guid reservationId, [FromBody] ReservationReasonRequestDto request)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var result = await _reservationService.SelfCancelAsync(reservationId, reason: request.Reason, cancelledById: (Guid)userId);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return NoContent();
        }


        [HttpPost("{reservationId:guid}/force-cancel")]
        [RequirePermission(Permissions.ReservationForceCancel)]
        public async Task<ActionResult> ForceCancel(Guid reservationId, [FromBody] ReservationReasonRequestDto request)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var result = await _reservationService.ForceCancelAsync(reservationId, reason: request.Reason, cancelledById: (Guid)userId);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return NoContent();
        }

        [HttpDelete("{reservationId:guid}")]
        [RequireCompletedProfile]
        public async Task<ActionResult> Delete(Guid reservationId)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var result = await _reservationService.DeleteAsync(reservationId, requestingUserId: (Guid)userId);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return NoContent();
        }

        [HttpPut("{reservationId:guid}")]
        public async Task<ActionResult<ReservationResponseDto>> Update(Guid reservationId, [FromBody] UpdateReservationRequestDto request)
        {
            var userId = User.GetUserId();
            if (userId == null)
                return Unauthorized();

            var result = await _reservationService.UpdateAsync(
                requestingUserId: (Guid)userId,
                reservationId: reservationId,
                startTime: request.StartTime,
                endTime: request.EndTime,
                purpose: request.Purpose
            );
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return Ok(result.Value.ToBasicDto());
        }
    }
}
