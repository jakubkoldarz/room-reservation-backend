using RoomReservation.Api.Dtos;
using RoomReservation.Core.Models;

namespace RoomReservation.Api.Extensions.Mappers
{
    public static class ResultMapperExtensions
    {
        public static PagedResponseDto<TTarget> ToPagedDto<TSource, TTarget>(this PagedList<TSource> source, Func<TSource, TTarget> mapper)
        {
            return new PagedResponseDto<TTarget>
            {
                Items = [.. source.Items.Select(mapper)],
                TotalCount = source.TotalCount,
                Page = source.Page,
                PageSize = source.PageSize,
                TotalPages = source.TotalPages,
                HasNextPage = source.HasNextPage
            };
        }
    }
}
