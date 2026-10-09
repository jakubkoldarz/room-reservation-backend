using Microsoft.EntityFrameworkCore;
using RoomReservation.Core.Filters;
using RoomReservation.Core.Models;

namespace RoomReservation.Core.Extensions
{
    internal static class QueryableExtensions
    {
        public static async Task<PagedList<T>> ToPagedListAsync<T>(this IQueryable<T> query, PagedFilter filters)
        {
            var totalCount = await query.CountAsync();
            var items = await query
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            return new PagedList<T>(items, totalCount, filters.Page, filters.PageSize);
        }
    }
}
