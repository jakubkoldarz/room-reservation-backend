namespace RoomReservation.Core.Filters
{
    public class BuildingFilter : PagedFilter
    {
        public string? Name { get; set; }
        public string? Identifier { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
    }
}
