using Atlas.Template.Core.Interfaces;
using Atlas.Template.Core.Options;
using Atlas.Template.Services.IServices;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using System.Threading.Tasks;

namespace Atlas.Template.Services.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailOptions _emailOptions;
        public EmailService(IOptions<EmailOptions> emailOptions)
        {
            _emailOptions = emailOptions.Value;
        }

        public async Task SendAsync(IEmailStructure email)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(_emailOptions.SenderName, _emailOptions.SenderEmail));
            message.To.Add(new MailboxAddress(email.RecipientName, email.To));

            message.Subject = email.Subject;
            message.Body = email.IsHtml ? new TextPart("html") { Text = email.Body } : new TextPart("plain") { Text = email.Body };

            using var smtpClient = new SmtpClient();
            await smtpClient.ConnectAsync(_emailOptions.Host, _emailOptions.Port, SecureSocketOptions.StartTls);
            await smtpClient.AuthenticateAsync(_emailOptions.SenderEmail, _emailOptions.Password);
            await smtpClient.SendAsync(message);
            await smtpClient.DisconnectAsync(true); 
        }
    }
}
