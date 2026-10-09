using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Core.Filters
{
    public abstract class PagedFilter
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page must be a positive integer.")]
        public int Page { get; set; } = 1;
        [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
        public int PageSize { get; set; } = 10;
    }
}
