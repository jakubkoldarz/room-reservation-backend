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
using RoomReservation.Core.Models.Availability;
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
        public async Task<ActionResult<PagedResult<BasicRoomResponseDto>>> GetAll([FromQuery] RoomFilter filters)
        {
            var result = await _roomService.GetAllAsync(filters);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return Ok(result.ToDto(r => r.ToBasicDto()));
        }

        [RequirePermission(Permissions.RoomView)]
        [HttpGet("{roomId:guid}")]
        public async Task<ActionResult<BasicRoomResponseDto>> GetSingle([FromRoute] Guid roomId)
        {
            var result = await _roomService.GetByIdAsync(roomId);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return Ok(result.Value.ToDetailsDto());
        }

        [RequirePermission(Permissions.RoomAdd)]
        [HttpPost]
        public async Task<ActionResult<BasicRoomResponseDto>> Create([FromBody] RoomRequestDto request)
        {
            var roomRequest = ToRoomRequest(request);

            var result = await _roomService.CreateAsync(roomRequest);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return CreatedAtAction(nameof(GetSingle), new { roomId = result.Value.Id }, result.Value.ToBasicDto());
        }

        [RequirePermission(Permissions.RoomEdit)]
        [HttpPut("{roomId:guid}")]
        public async Task<ActionResult<BasicRoomResponseDto>> Update(
            [FromRoute] Guid roomId,
            [FromBody] RoomRequestDto request,
            [FromQuery] bool force = false)
        {
            var roomRequest = ToRoomRequest(request);

            var result = await _roomService.UpdateAsync(roomId, roomRequest, force);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return Ok(result.Value.ToBasicDto());
        }

        [RequirePermission(Permissions.RoomDelete)]
        [HttpDelete("{roomId:guid}")]
        public async Task<ActionResult> Delete([FromRoute] Guid roomId, [FromQuery] bool force = false)
        {
            var result = await _roomService.DeleteAsync(roomId, force);
            if (!result.IsSuccess)
                return result.Error.ToActionResult();

            return NoContent();
        }

        private static RoomModel ToRoomRequest(RoomRequestDto request)
        {
            return new RoomModel(
                Identifier: request.Identifier,
                RequiresApproval: request.RequiresApproval,
                BuildingId: request.BuildingId,
                Floor: request.Floor,
                Capacity: request.Capacity,
                EquipmentIds: request.EquipmentIds,
                Availabilities: [.. request.Availabilities.Select(a => new AvailabilityModel(
                    DayOfWeek: a.DayOfWeek,
                    StartTime: a.StartTime,
                    EndTime: a.EndTime))]
            );
        }
    }
}