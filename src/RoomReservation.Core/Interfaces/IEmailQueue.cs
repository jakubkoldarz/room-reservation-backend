using RoomReservation.Core.Emails;

namespace RoomReservation.Core.Interfaces
{
    public interface IEmailQueue
    {
        void Enqueue(EmailMessage message);
    }
}
