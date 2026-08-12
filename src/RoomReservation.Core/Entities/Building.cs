namespace RoomReservation.Core.Entities
{
    public class Building
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public required string Name { get; set; }
        public string? Identifier { get; set; }
        public required string Street { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
        public required int FloorsCount { get; set; }

        public ICollection<Room> Rooms { get; set; } = [];
        public ICollection<BuildingAvailability> Availabilities { get; set; } = [];
        public ICollection<BuildingSpecialAvailability> SpecialAvailabilities { get; set; } = [];
    }
}
