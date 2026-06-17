using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Results
{
    public class PagedResult<T> : IResult<IEnumerable<T>>
    {
        public bool IsSuccess { get; init; }
        public IEnumerable<T>? Value { get; init; }
        public string? ErrorMessage { get; init; }

        public int TotalCount { get; init; }
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => Page < TotalPages;

        public static PagedResult<T> Success(IEnumerable<T> items, int totalCount, int page, int pageSize)
        {
            return new PagedResult<T>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Value = items,
                IsSuccess = true
            };
        }

        public static PagedResult<T> Failure(string errorMessage)
        {
            return new PagedResult<T>
            {
                ErrorMessage = errorMessage,
                IsSuccess = false
            };
        }
    }
}
