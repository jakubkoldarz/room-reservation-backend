using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
    [EnableRateLimiting("default")]
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class EventsController(IEventService eventService) : ControllerBase
    {
        [RequirePermission(Permissions.EventView)]
        [HttpGet("{eventId:guid}")]
        public async Task<ActionResult<EventResponseDto>> GetSingle([FromRoute] Guid eventId)
        {
            var result = await eventService.GetByIdAsync(eventId);
            return result.ToActionResult(ev => Ok(ev.ToDto()));
        }

        [RequirePermission(Permissions.EventView)]
        [HttpGet("rooms/{roomId:guid}")]
        public async Task<ActionResult<IReadOnlyList<EventResponseDto>>> GetActiveForRoom([FromRoute] Guid roomId)
        {
            var events = await eventService.GetActiveForRoomAsync(roomId);
            return Ok(events.Select(e => e.ToDto()).ToList());
        }

        [RequirePermission(Permissions.EventAdd)]
        [HttpPost]
        public async Task<ActionResult<EventResponseDto>> Create([FromBody] EventRequestDto request, [FromQuery] bool force = false)
        {
            var result = await eventService.CreateAsync(request.RoomIds, ToEventRequest(request), force);
            return result.ToActionResult(ev =>
                CreatedAtAction(nameof(GetSingle), new { eventId = ev.Id }, ev.ToDto()));
        }

        [RequirePermission(Permissions.EventEdit)]
        [HttpPut("{eventId:guid}")]
        public async Task<ActionResult<EventResponseDto>> Update(
            [FromRoute] Guid eventId,
            [FromBody] EventRequestDto request,
            [FromQuery] bool force = false)
        {
            var result = await eventService.UpdateAsync(eventId, request.RoomIds, ToEventRequest(request), force);
            return result.ToActionResult(ev => Ok(ev.ToDto()));
        }

        [RequirePermission(Permissions.EventDelete)]
        [HttpDelete("{eventId:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid eventId, [FromQuery] bool force = false)
        {
            var result = await eventService.DeleteAsync(eventId, force);
            return result.ToActionResult(NoContent);
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
