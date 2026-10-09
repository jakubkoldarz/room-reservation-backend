using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Data
{
    public class UnitOfWork(AppDbContext db) : IUnitOfWork
    {
        public async Task SaveChangesAsync()
            => await db.SaveChangesAsync();

        public async Task<Result> ExecuteInTransactionAsync(Func<Task<Result>> action)
        {
            await using var transaction = await db.Database.BeginTransactionAsync();

            var result = await action();
            if (!result.IsSuccess)
                return result;

            await db.SaveChangesAsync();
            await transaction.CommitAsync();
            return result;
        }

        public async Task<ResultT<T>> ExecuteInTransactionAsync<T>(Func<Task<ResultT<T>>> action)
        {
            await using var transaction = await db.Database.BeginTransactionAsync();

            var result = await action();
            if (!result.IsSuccess)
                return result;

            await db.SaveChangesAsync();
            await transaction.CommitAsync();
            return result;
        }
    }
}
