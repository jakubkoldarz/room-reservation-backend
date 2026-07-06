using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Models;
using RoomReservation.Core.Results.Common;

namespace RoomReservation.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpEmail;
        private readonly string _smtpPassword;
        private readonly string _templatesPath;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration config, IWebHostEnvironment env, ILogger<EmailService> logger)
        {
            _smtpHost = config["SMTP:Host"] ?? throw new InvalidOperationException("Missing config: SMTP:Host");
            var portString = config["SMTP:Port"] ?? throw new InvalidOperationException("Missing config: SMTP:Port");
            if(!int.TryParse(portString, out _smtpPort))
                throw new InvalidOperationException("Invalid config: SMTP:Port must be integer");

            _smtpEmail = config["SMTP:Email"] ?? throw new InvalidOperationException("Missing config: SMTP:Email");
            _smtpPassword = config["SMTP:Password"] ?? throw new InvalidOperationException("Missing config: SMTP:Password");

            _templatesPath = Path.Combine(env.ContentRootPath, "Templates");

            _logger = logger;
        }

        public async Task<ResultT<string>> GetMessageAsync(string template, Dictionary<string, string> replacements)
        {
            var path = Path.Combine(_templatesPath, template);

            if (!File.Exists(path))
                return new Error("Template file does not exist", ErrorType.Internal);

            var html = await File.ReadAllTextAsync(path);

            foreach (var (key, value) in replacements)
            {
                html = html.Replace("{{" + key + "}}", value);
            }

            return ResultT<string>.Success(html);
        }

        public async Task<Result> SendEmailAsync(EmailMessage message)
        {
            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("Sender", _smtpEmail));
            mimeMessage.To.Add(new MailboxAddress("Sender", message.To));
            mimeMessage.Subject = message.Subject;
            mimeMessage.Body = new TextPart("html") { Text = message.HtmlMessage };

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
    }
}
