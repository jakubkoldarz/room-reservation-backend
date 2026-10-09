using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Core.Models.Availability
{
    public record AvailabilityModel
    (
        [Required] DayOfWeek DayOfWeek,
        [Required] TimeOnly StartTime,
        [Required] TimeOnly EndTime
    )
    {
        public Entities.Availability ToEntity(Guid? roomId = null, Guid? buildingId = null) => new()
        {
            RoomId = roomId,
            BuildingId = buildingId,
            DayOfWeek = DayOfWeek,
            StartTime = StartTime,
            EndTime = EndTime
        };
    }
}
