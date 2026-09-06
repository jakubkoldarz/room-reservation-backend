using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using RoomReservation.Core.Emails;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;
using RoomReservation.Core.Results.Common;
using System.Reflection;

namespace RoomReservation.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpEmail;
        private readonly string _smtpPassword;
        private readonly IJobService _jobService;
        private readonly ILogger<EmailService> _logger;

        private static readonly Assembly _assembly = typeof(EmailService).Assembly;

        public EmailService(IConfiguration config, IJobService jobService, ILogger<EmailService> logger)
        {
            _smtpHost = config["SMTP:Host"] ?? throw new InvalidOperationException("Missing config: SMTP:Host");
            var portString = config["SMTP:Port"] ?? throw new InvalidOperationException("Missing config: SMTP:Port");
            if (!int.TryParse(portString, out _smtpPort))
                throw new InvalidOperationException("Invalid config: SMTP:Port must be integer");

            _smtpEmail = config["SMTP:Email"] ?? throw new InvalidOperationException("Missing config: SMTP:Email");
            _smtpPassword = config["SMTP:Password"] ?? throw new InvalidOperationException("Missing config: SMTP:Password");

            _jobService = jobService;
            _logger = logger;
        }

        public async Task<Result> EnqueueEmailAsync(EmailMessage message)
        {
            var job = new JobModel(JobTypes.SendEmail, message.ToJsonPayload());

            try
            {
                await _jobService.EnqueueJobAsync(job);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to enqueue email job for {Recipient}", message.To);
                return Result.Failure("Nie udało się zakolejkować wiadomości email", ErrorType.Internal);
            }

            return Result.Success();
        }

        public async Task<Result> SendEmailAsync(EmailMessage message)
        {
            var renderResult = await RenderTemplateAsync(message.TemplateName, message.GetReplacements());
            if (!renderResult.IsSuccess)
                return renderResult.Error;

            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("RoomReservation", _smtpEmail));
            mimeMessage.To.Add(new MailboxAddress(message.To, message.To));
            mimeMessage.Subject = message.Subject;
            mimeMessage.Body = new TextPart("html") { Text = renderResult.Value };

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(_smtpHost, _smtpPort, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_smtpEmail, _smtpPassword);
                await client.SendAsync(mimeMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Recipient}", message.To);
                return Result.Failure("Email Exception", ErrorType.BadRequest);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }

            return Result.Success();
        }

        private async Task<ResultT<string>> RenderTemplateAsync(string templateName, Dictionary<string, string> replacements)
        {
            var resourceName = $"{typeof(EmailService).Namespace}.Emails.Templates.{templateName}.html";

            using var stream = _assembly.GetManifestResourceStream(resourceName);
            if (stream is null)
            {
                _logger.LogError("Template not found as embedded resource: {ResourceName}", resourceName);
                return new Error("Template file does not exist", ErrorType.Internal);
            }

            using var reader = new StreamReader(stream);
            var html = await reader.ReadToEndAsync();

            foreach (var (key, value) in replacements)
            {
                html = html.Replace("{{" + key + "}}", value);
            }

            return ResultT<string>.Success(html);
        }
    }
}