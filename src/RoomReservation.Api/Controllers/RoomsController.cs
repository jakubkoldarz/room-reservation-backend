using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using RoomReservation.Api.Attributes;
using RoomReservation.Api.Dtos;
using RoomReservation.Api.Dtos.Rooms.Requests;
using RoomReservation.Api.Dtos.Rooms.Responses;
using RoomReservation.Api.Extensions;
using RoomReservation.Api.Extensions.Mappers;
using RoomReservation.Core.Constants;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models.Availability;
using RoomReservation.Core.Models.Rooms;

namespace RoomReservation.Api.Controllers
{
    [Authorize]
    [EnableRateLimiting("default")]
    [Route("[controller]")]
    [ApiController]
    public class RoomsController(IRoomService roomService) : ControllerBase
    {
        [RequirePermission(Permissions.RoomList)]
        [HttpGet]
        public async Task<ActionResult<PagedResponseDto<BasicRoomResponseDto>>> GetAll([FromQuery] RoomFilter filters)
        {
            var result = await roomService.GetAllAsync(filters);
            return result.ToActionResult(page => Ok(page.ToPagedDto(r => r.ToBasicDto())));
        }

        [RequirePermission(Permissions.RoomView)]
        [HttpGet("{roomId:guid}")]
        public async Task<ActionResult<RoomDetailsResponseDto>> GetSingle([FromRoute] Guid roomId)
        {
            var result = await roomService.GetByIdAsync(roomId);
            return result.ToActionResult(room => Ok(room.ToDetailsDto()));
        }

        [RequirePermission(Permissions.RoomAdd)]
        [HttpPost]
        public async Task<ActionResult<BasicRoomResponseDto>> Create([FromBody] RoomRequestDto request)
        {
            var result = await roomService.CreateAsync(ToRoomRequest(request));
            return result.ToActionResult(room =>
                CreatedAtAction(nameof(GetSingle), new { roomId = room.Id }, room.ToBasicDto()));
        }

        [RequirePermission(Permissions.RoomEdit)]
        [HttpPut("{roomId:guid}")]
        public async Task<ActionResult<BasicRoomResponseDto>> Update(
            [FromRoute] Guid roomId,
            [FromBody] RoomRequestDto request,
            [FromQuery] bool force = false)
        {
            var result = await roomService.UpdateAsync(roomId, ToRoomRequest(request), force);
            return result.ToActionResult(room => Ok(room.ToBasicDto()));
        }

        [RequirePermission(Permissions.RoomDelete)]
        [HttpDelete("{roomId:guid}")]
        public async Task<IActionResult> Delete([FromRoute] Guid roomId, [FromQuery] bool force = false)
        {
            var result = await roomService.DeleteAsync(roomId, force);
            return result.ToActionResult(NoContent);
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
