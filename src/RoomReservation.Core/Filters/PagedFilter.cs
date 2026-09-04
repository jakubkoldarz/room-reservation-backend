using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Core.Filters
{
    public abstract class PagedFilter
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page must be a positive integer.")]
        public int Page { get; set; } = 1;
        [Range(1, int.MaxValue, ErrorMessage = "PageSize must be a positive integer.")]
        public int PageSize { get; set; } = 10;
    }
}
