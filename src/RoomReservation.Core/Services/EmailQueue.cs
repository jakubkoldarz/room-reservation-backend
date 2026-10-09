using RoomReservation.Core.Emails;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;

namespace RoomReservation.Core.Services
{
    public class EmailQueue(IJobService jobService) : IEmailQueue
    {
        public void Enqueue(EmailMessage message)
        {
            var job = new JobModel(JobTypes.SendEmail, message.ToJsonPayload());
            jobService.Enqueue(job);
        }
    }
}
