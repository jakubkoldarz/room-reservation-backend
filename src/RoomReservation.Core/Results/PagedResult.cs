using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace RoomReservation.Core.Results
{
    public class PagedResult<T> : IResultT<IEnumerable<T>>
    {
        [MemberNotNullWhen(true, nameof(Value))]
        [MemberNotNullWhen(false, nameof(Error))]
        public bool IsSuccess { get; init; }
        public IEnumerable<T>? Value { get; init; }
        public Error? Error { get; init; }

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

        public static PagedResult<T> Failure(string errorMessage, ErrorType errorType)
        {
            return new PagedResult<T>
            {
                Error = new(errorMessage, errorType),
                IsSuccess = false
            };
        }
    }
}
