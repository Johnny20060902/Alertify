using Alertify.Data;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Alertify.Services.Email
{
    public class EmailService
    {
        private readonly EmailOptions _emailOptions;
        private readonly AlertifyDbContext _context;

        public EmailService(IOptions<EmailOptions> emailOptions, AlertifyDbContext context)
        {
            _emailOptions = emailOptions.Value;
            _context = context;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailOptions.SenderName, _emailOptions.SenderEmail));
                message.To.Add(MailboxAddress.Parse(toEmail));
                message.Subject = subject;
                message.Body = new TextPart(MimeKit.Text.TextFormat.Html)
                {
                    Text = htmlBody
                };

                using (var client = new SmtpClient())
                {
                    await client.ConnectAsync(
                        _emailOptions.SmtpHost,
                        _emailOptions.SmtpPort,
                        SecureSocketOptions.StartTls
                    );
                    await client.AuthenticateAsync(
                        _emailOptions.Username,
                        _emailOptions.Password
                    );
                    await client.SendAsync(message);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    $"Error al enviar email a {toEmail}: {ex.Message}",
                    ex
                );
            }
        }
    }
}