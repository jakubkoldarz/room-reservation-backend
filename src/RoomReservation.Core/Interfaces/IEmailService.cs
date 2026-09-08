using RoomReservation.Core.Emails;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Interfaces
{
    public interface IEmailService
    {
        Task<Result> EnqueueEmailAsync(EmailMessage message);
        Task<Result> SendEmailAsync(EmailMessage message);
    }
}
