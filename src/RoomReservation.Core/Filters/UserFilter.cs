namespace RoomReservation.Core.Filters
{
    public class UserFilter : PagedFilter
    {
        public string? Firstname { get; set; }
        public string? Lastname { get; set; }
        public string? Email { get; set; }
    }
}
