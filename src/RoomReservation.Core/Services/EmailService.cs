using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using RoomReservation.Core.Emails;
using RoomReservation.Core.Enums;
using RoomReservation.Core.Interfaces;
using RoomReservation.Core.Options;
using RoomReservation.Core.Results.Common;
using System.Net;
using System.Reflection;

namespace RoomReservation.Core.Services
{
    public class EmailService(IOptions<SmtpOptions> smtpOptions, ILogger<EmailService> logger) : IEmailService
    {
        private static readonly Assembly _assembly = typeof(EmailService).Assembly;

        public async Task<Result> SendEmailAsync(EmailMessage message)
        {
            var smtp = smtpOptions.Value;

            var renderResult = await RenderTemplateAsync(message.TemplateName, message.GetReplacements());
            if (!renderResult.IsSuccess)
                return renderResult.Error;

            var mimeMessage = new MimeMessage();
            mimeMessage.From.Add(new MailboxAddress("RoomReservation", smtp.Email));
            mimeMessage.To.Add(new MailboxAddress(message.To, message.To));
            mimeMessage.Subject = message.Subject;
            mimeMessage.Body = new TextPart("html") { Text = renderResult.Value };

            using var client = new SmtpClient();
            try
            {
                await client.ConnectAsync(smtp.Host, smtp.Port, SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(smtp.Email, smtp.Password);
                await client.SendAsync(mimeMessage);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to send email to {Recipient}", message.To);
                return new Error("Email Exception", ErrorType.Internal);
            }
            finally
            {
                await client.DisconnectAsync(true);
            }

            return Result.Success();
        }

        private async Task<ResultT<string>> RenderTemplateAsync(string templateName, Dictionary<string, string> replacements)
        {
            var resourceName = $"{typeof(EmailMessage).Namespace}.Templates.{templateName}.html";

            using var stream = _assembly.GetManifestResourceStream(resourceName);
            if (stream is null)
            {
                logger.LogError("Template not found as embedded resource: {ResourceName}", resourceName);
                return new Error("Template file does not exist", ErrorType.Internal);
            }

            using var reader = new StreamReader(stream);
            var html = await reader.ReadToEndAsync();

            foreach (var (key, value) in replacements)
            {
                html = html.Replace("{{" + key + "}}", WebUtility.HtmlEncode(value));
            }

            return ResultT<string>.Success(html);
        }
    }
}
