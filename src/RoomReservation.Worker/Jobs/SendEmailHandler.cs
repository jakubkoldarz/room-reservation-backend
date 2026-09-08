using RoomReservation.Core.Emails;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Results.Common;
using System.Text.Json;

namespace RoomReservation.Worker.Jobs
{
    internal class SendEmailHandler(IEmailService _emailService) : IJobHandler
    {
        private static readonly Dictionary<string, Type> _registry = typeof(EmailMessage).Assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(EmailMessage)) && !t.IsAbstract)
                .ToDictionary(t => t.Name);

        public JobTypes JobType => JobTypes.SendEmail;
        public async Task<Result> HandleAsync(string payload, CancellationToken ct)
        {
            var message = DeserializeMessage(payload);
            var result = await _emailService.SendEmailAsync(message);
            if (!result.IsSuccess)
            {
                throw new InvalidOperationException(result.Error.ErrorMessage);
            }
            return Result.Success();
        }
        private static EmailMessage DeserializeMessage(string payload)
        {
            var wrapper = JsonSerializer.Deserialize<EmailJobPayload>(payload)
                ?? throw new InvalidOperationException("Nieprawidłowy payload joba email.");

            if (!_registry.TryGetValue(wrapper.EmailType, out var type))
                throw new InvalidOperationException($"Nieznany typ maila: {wrapper.EmailType}");

            return (EmailMessage)JsonSerializer.Deserialize(wrapper.Data, type)!;
        }
    }
}
