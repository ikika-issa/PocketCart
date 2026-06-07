using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using PocketCartApp.Domain.Email;
using PocketCartApp.Service.Interface;
using System.Net.Mail;

namespace PocketCartApp.Service.Implementation
{
    public class EmailService : IEmailService, IEmailSender
    {
        private readonly MailSettings _mailSettings;

        public EmailService(IOptions<MailSettings> mailSettings)
        {
            _mailSettings = mailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new EmailMessage
            {
                MailTo = email,
                Subject = subject,
                Content = htmlMessage
            };

            await SendEmailAsync(message);
        }

        public async Task SendEmailAsync(EmailMessage message)
        {
            var email = new MimeMessage();

            email.From.Add(new MailboxAddress(
                _mailSettings.SendersName,
                _mailSettings.SmtpUserName!));

            email.To.Add(new MailboxAddress(
                message.MailTo,
                message.MailTo!));

            email.Subject = message.Subject!;

            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message.Content!
            };

            try
            {
                using var smtp = new MailKit.Net.Smtp.SmtpClient();

                await smtp.ConnectAsync(
                    _mailSettings.SmtpServer!,
                    _mailSettings.SmtpServerPort,
                    SecureSocketOptions.StartTls
);

                if (!string.IsNullOrEmpty(_mailSettings.SmtpUserName))
                {
                    await smtp.AuthenticateAsync(
                        _mailSettings.SmtpUserName,
                        _mailSettings.SmtpPassword!);
                }

                await smtp.SendAsync(email);
                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                throw new Exception("Email sending failed", ex);
            }
        }
    }
}