using RoomReservation.Core.Entities;
using RoomReservation.Core.Results.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Interfaces
{
    public interface IBuildingService
    {
        Task<ResultT<Building>> GetByIdAsync(Guid buildingId);
        Task<ResultT<IReadOnlyList<Building>>> GetAllAsync();
        Task<ResultT<Building>> CreateAsync(string name, string? identifier, string street, string city, string postalCode, int floorsCount);
        Task<Result> UpdateAsync(Guid buildingId, string name, string? identifier, string street, string city, string postalCode, int floorsCount);
        Task<Result> DeleteAsync(Guid buildingId);
    }
}
