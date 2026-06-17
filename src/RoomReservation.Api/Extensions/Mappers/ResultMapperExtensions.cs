using RoomReservation.Core.Results;

namespace RoomReservation.Api.Extensions.Mappers
{
    public static class ResultMapperExtensions
    {
        public static PagedResult<TTarget> ToDto<TSource, TTarget>(this PagedResult<TSource> source, Func<TSource, TTarget> mapper)
        {
            if (!source.IsSuccess) 
                throw new InvalidOperationException("Attempt to convert invalid result");

            return PagedResult<TTarget>.Success(
                source.Value!.Select(mapper),
                source.TotalCount,
                source.Page,
                source.PageSize
            );
        }
    }
}
