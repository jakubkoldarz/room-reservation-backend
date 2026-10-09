using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Data
{
    public class UnitOfWork(AppDbContext _db) : IUnitOfWork
    {
        public async Task SaveChangesAsync()
            => await _db.SaveChangesAsync();

        public async Task<Result> ExecuteInTransactionAsync(Func<Task<Result>> action)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();

            var result = await action();
            if (!result.IsSuccess)
                return result;

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            return result;
        }

        public async Task<ResultT<T>> ExecuteInTransactionAsync<T>(Func<Task<ResultT<T>>> action)
        {
            await using var transaction = await _db.Database.BeginTransactionAsync();

            var result = await action();
            if (!result.IsSuccess)
                return result;

            await _db.SaveChangesAsync();
            await transaction.CommitAsync();
            return result;
        }
    }
}
