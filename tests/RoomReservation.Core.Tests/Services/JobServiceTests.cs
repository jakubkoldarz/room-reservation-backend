using FluentAssertions;
using Moq;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;
using RoomReservation.Core.Services;
using RoomReservation.Core.Tests.TestHelpers;

namespace RoomReservation.Core.Tests.Services
{
    public class JobServiceTests
    {
        private readonly Mock<IJobRepository> _jobsMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();

        private readonly JobService _sut;

        private static readonly DateTime Now = FixedTimeProvider.DefaultNow.UtcDateTime;

        public JobServiceTests()
        {
            _sut = new(_jobsMock.Object, _unitOfWorkMock.Object, new FixedTimeProvider());
        }

        private static Job CreateJob(int attempts = 1) => new()
        {
            JobType = JobTypes.SendEmail,
            Payload = "{}",
            Status = JobStatus.Processing,
            Attempts = attempts,
            NextAttemptAt = Now
        };

        [Fact]
        public void Enqueue_WithoutDelay_SchedulesJobForNowWithoutSaving()
        {
            _sut.Enqueue(new JobModel(JobTypes.SendEmail, "{}"));

            _jobsMock.Verify(x => x.Add(It.Is<Job>(j =>
                j.JobType == JobTypes.SendEmail &&
                j.Status == JobStatus.Pending &&
                j.NextAttemptAt == Now)), Times.Once);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public void Enqueue_WithDelay_SchedulesJobInTheFuture()
        {
            _sut.Enqueue(new JobModel(JobTypes.SendEmail, "{}", Delay: TimeSpan.FromMinutes(5)));

            _jobsMock.Verify(x => x.Add(It.Is<Job>(j => j.NextAttemptAt == Now.AddMinutes(5))), Times.Once);
        }

        [Fact]
        public async Task MarkAsCompletedAsync_SetsStatusAndProcessedAt()
        {
            var job = CreateJob();

            await _sut.MarkAsCompletedAsync(job);

            job.Status.Should().Be(JobStatus.Completed);
            job.ProcessedAt.Should().Be(Now);
            _unitOfWorkMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Theory]
        [InlineData(1, 30)]
        [InlineData(2, 60)]
        [InlineData(3, 120)]
        public async Task MarkAsFailedAsync_SchedulesRetryWithExponentialBackoff(int attempts, int expectedDelaySeconds)
        {
            var job = CreateJob(attempts);

            await _sut.MarkAsFailedAsync(job, "SMTP down");

            job.Status.Should().Be(JobStatus.Failed);
            job.LastError.Should().Be("SMTP down");
            job.NextAttemptAt.Should().Be(Now.AddSeconds(expectedDelaySeconds));
        }

        [Fact]
        public async Task TryClaimNextJobAsync_WhenQueueIsEmpty_ReturnsNotFound()
        {
            _jobsMock.Setup(x => x.TryClaimNextJobAsync()).ReturnsAsync((Job?)null);

            var result = await _sut.TryClaimNextJobAsync();

            result.Error!.ErrorType.Should().Be(ErrorType.NotFound);
        }
    }
}
