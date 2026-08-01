namespace RoomReservation.Core.Entities
{
    public class Equipment
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();
        public required string Name { get; set; }
        public required string Icon { get; set; }
    }
}
