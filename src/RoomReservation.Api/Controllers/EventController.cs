using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomReservation.Api.Attributes;
using RoomReservation.Api.Dtos.Events.Requests;
using RoomReservation.Api.Dtos.Events.Responses;
using RoomReservation.Api.Extensions;
using RoomReservation.Api.Extensions.Mappers;
using RoomReservation.Core.Constants;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models.Events;

namespace RoomReservation.Api.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class EventsController(IEventService _eventService) : ControllerBase
    {
        [RequirePermission(Permissions.EventView)]
        [HttpGet("{eventId:guid}")]
        public async Task<ActionResult<EventResponseDto>> GetSingle([FromRoute] Guid eventId)
        {
            var ev = await _eventService.GetByIdAsync(eventId);
            if (!ev.IsSuccess)
                return ev.Error.ToActionResult();

            return Ok(ev.Value.ToDto());
        }

        [RequirePermission(Permissions.EventView)]
        [HttpGet("rooms/{roomId:guid}")]
        public async Task<ActionResult<IReadOnlyList<EventResponseDto>>> GetActiveForRoom([FromRoute] Guid roomId)
        {
            var events = await _eventService.GetActiveForRoomAsync(roomId);
            return Ok(events.Select(e => e.ToDto()).ToList());
        }

        [RequirePermission(Permissions.EventAdd)]
        [HttpPost]
        public async Task<ActionResult<EventResponseDto>> Create([FromBody] EventRequestDto request, [FromQuery] bool force = false)
        {
            var eventRequest = ToEventRequest(request);

            var result = await _eventService.CreateAsync(request.RoomIds, eventRequest, force);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return CreatedAtAction(nameof(GetSingle), new { eventId = result.Value.Id }, result.Value.ToDto());
        }

        [RequirePermission(Permissions.EventEdit)]
        [HttpPut("{eventId:guid}")]
        public async Task<ActionResult<EventResponseDto>> Update(
            [FromRoute] Guid eventId,
            [FromBody] EventRequestDto request,
            [FromQuery] bool force = false)
        {
            var eventRequest = ToEventRequest(request);

            var result = await _eventService.UpdateAsync(eventId, request.RoomIds, eventRequest, force);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return Ok(result.Value.ToDto());
        }

        [RequirePermission(Permissions.EventDelete)]
        [HttpDelete("{eventId:guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid eventId, [FromQuery] bool force = false)
        {
            var result = await _eventService.DeleteAsync(eventId, force);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return NoContent();
        }

        private static EventModel ToEventRequest(EventRequestDto request)
        {
            return new EventModel(
                Name: request.Name,
                StartDate: request.StartDate,
                EndDate: request.EndDate,
                IsClosed: request.IsClosed,
                StartTime: request.StartTime,
                EndTime: request.EndTime
            );
        }
    }
}