namespace RoomReservation.Core.Models
{
    public class PagedList<T>(IReadOnlyList<T> items, int totalCount, int page, int pageSize)
    {
        public IReadOnlyList<T> Items { get; } = items;
        public int TotalCount { get; } = totalCount;
        public int Page { get; } = page;
        public int PageSize { get; } = pageSize;
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => Page < TotalPages;
    }
}
