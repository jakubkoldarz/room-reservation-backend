using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RoomReservation.Api.Attributes;
using RoomReservation.Api.Dtos.Rooms.Requests;
using RoomReservation.Api.Dtos.Rooms.Responses;
using RoomReservation.Api.Extensions;
using RoomReservation.Api.Extensions.Mappers;
using RoomReservation.Core.Constants;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Api.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class RoomsController(IRoomService _roomService) : ControllerBase
    {
        [RequirePermission(Permissions.RoomList)]
        [HttpGet]
        public async Task<ActionResult<PagedResult<BasicRoomResponse>>> GetAll([FromQuery] RoomFilter filters)
        {
            var result = await _roomService.GetAllAsync(filters);
            if(!result.IsSuccess)
                return result.Error.ToActionResult();
    
            return Ok(result.ToDto(r => r.ToBasicDto()));
        }

        [RequirePermission(Permissions.RoomView)]
        [HttpGet("{roomId:guid}")]
        public async Task<ActionResult<BasicRoomResponse>> GetSingle([FromRoute] Guid roomId)
        {
            var result = await _roomService.GetByIdAsync(roomId);
            if(!result.IsSuccess)
                return result.Error.ToActionResult();

            return Ok(result.Value.ToDetailsDto());
        }

        [RequirePermission(Permissions.RoomAdd)]
        [HttpPost]
        public async Task<ActionResult<BasicRoomResponse>> Create([FromBody] RoomRequest request)
        {
            var result = await _roomService.CreateAsync(
                request.Identifier,
                request.RequiresApproval,
                request.BuildingId,
                request.Floor,
                request.Capacity,
                request.EquipmentIds,
                [.. request.Availabilities.Select(a => new AvailabilitySlot(a.DayOfWeek, a.StartTime, a.EndTime))]);

            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return CreatedAtAction(nameof(GetSingle), new { roomId = result.Value.Id }, result.Value.ToBasicDto());
        }

        [RequirePermission(Permissions.RoomEdit)]
        [HttpPut("{roomId:guid}")]
        public async Task<ActionResult<BasicRoomResponse>> Update([FromRoute] Guid roomId, [FromBody] RoomRequest request)
        {
            var result = await _roomService.UpdateAsync(
                roomId,
                request.Identifier,
                request.RequiresApproval,
                request.BuildingId,
                request.Floor,
                request.Capacity,
                request.EquipmentIds,
                [.. request.Availabilities.Select(a => new AvailabilitySlot(a.DayOfWeek, a.StartTime, a.EndTime))]);

            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return Ok(result.Value.ToBasicDto());
        }

        [RequirePermission(Permissions.RoomDelete)]
        [HttpDelete("{roomId:guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid roomId)
        {
            var result = await _roomService.DeleteAsync(roomId);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return NoContent();
        }
    }
}
