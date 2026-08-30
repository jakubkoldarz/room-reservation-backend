namespace RoomReservation.Core.Entities
{
    public class Room
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public required string Identifier { get; set; }
        public required bool RequiresApproval { get; set; }

        public Guid BuildingId { get; set; }
        public Building Building { get; set; } = null!;

        public required int Floor { get; set; }
        public required int Capacity { get; set; }

        public ICollection<RoomEquipment> RoomEquipment { get; set; } = [];
        public ICollection<Availability> Availabilities { get; set; } = [];
        public ICollection<Event> Events { get; set; } = [];
        public ICollection<Reservation> Reservations { get; set; } = [];
    }
}
