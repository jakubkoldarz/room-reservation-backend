using RoomReservation.Core.Emails;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Interfaces
{
    public interface IEmailService
    {
        void EnqueueEmail(EmailMessage message);
        Task<Result> SendEmailAsync(EmailMessage message);
    }
}
