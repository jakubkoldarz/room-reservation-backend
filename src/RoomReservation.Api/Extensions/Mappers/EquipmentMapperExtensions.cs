using RoomReservation.Api.Dtos.Equipment.Responses;
using RoomReservation.Core.Entities;

namespace RoomReservation.Api.Extensions.Mappers
{
    public static class EquipmentMapperExtensions
    {
        public static EquipmentResponseDto ToBasicDto(this Equipment equipment)
        {
            return new EquipmentResponseDto
            { 
                Id = equipment.Id,
                Name = equipment.Name,
                Icon = equipment.Icon
            };
        }
    }
}
