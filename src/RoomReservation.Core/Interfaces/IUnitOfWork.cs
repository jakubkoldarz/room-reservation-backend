using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Interfaces
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
        Task<Result> ExecuteInTransactionAsync(Func<Task<Result>> action);
        Task<ResultT<T>> ExecuteInTransactionAsync<T>(Func<Task<ResultT<T>>> action);
    }
}
