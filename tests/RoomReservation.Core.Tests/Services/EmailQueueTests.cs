using FluentAssertions;
using Moq;
using RoomReservation.Core.Emails;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;
using RoomReservation.Core.Services;
using System.Text.Json;

namespace RoomReservation.Core.Tests.Services
{
    public class EmailQueueTests
    {
        private readonly Mock<IJobService> _jobServiceMock = new();
        private readonly EmailQueue _sut;

        public EmailQueueTests()
        {
            _sut = new(_jobServiceMock.Object);
        }

        [Fact]
        public void Enqueue_CreatesSendEmailJobWithSerializedMessage()
        {
            JobModel? job = null;
            _jobServiceMock.Setup(x => x.Enqueue(It.IsAny<JobModel>())).Callback<JobModel>(j => job = j);

            _sut.Enqueue(new PasswordChangeEmail { To = "jan@test.com", Title = "Alert" });

            job.Should().NotBeNull();
            job!.JobType.Should().Be(JobTypes.SendEmail);

            var payload = JsonSerializer.Deserialize<EmailJobPayload>(job.Payload)!;
            payload.EmailType.Should().Be(nameof(PasswordChangeEmail));

            var message = JsonSerializer.Deserialize<PasswordChangeEmail>(payload.Data)!;
            message.To.Should().Be("jan@test.com");
            message.Title.Should().Be("Alert");
        }
    }
}
