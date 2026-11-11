using System.ComponentModel.DataAnnotations;

namespace RoomReservation.Core.Filters
{
    public abstract class PagedFilter
    {
        [Range(1, int.MaxValue, ErrorMessage = "Page must be a positive integer.")]
        public int Page { get; set; } = 1;
        [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
        public int PageSize { get; set; } = 10;
        [MaxLength(50, ErrorMessage = "SortBy can have at most 50 characters.")]
        public string? SortBy { get; set; }
        [RegularExpression("^(asc|desc)$", ErrorMessage = "SortDirection must be 'asc' or 'desc'.")]
        public string? SortDirection { get; set; }
    }
}
