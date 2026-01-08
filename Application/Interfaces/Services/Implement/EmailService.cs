

using Application.Common.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace Application.Interfaces.Services.Implement
{
    public class EmailService : IEmailService.IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }



        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            // create empty email 
            var email = new MimeMessage();
            // dedect the sender and receiver
            email.From.Add(new MailboxAddress(_emailSettings.SenderName,_emailSettings.SenderEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            // set subject
            email.Subject = subject;
            // build the body of the email
            // set body as html for better formatting
            var builder = new BodyBuilder { HtmlBody = body };
            email.Body = builder.ToMessageBody();
            

            using var client = new MailKit.Net.Smtp.SmtpClient();

            // connect to the smtp server
            try
            {
                _logger.LogInformation("start to send email to {Email}", toEmail);
                // connect and Authentication
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                await client.AuthenticateAsync(_emailSettings.Username, _emailSettings.Password);

                // send the email 
                await client.SendAsync(email);

                _logger.LogInformation("Email sent successfully to {Email}", toEmail);

            }

            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail); throw;
            }
            finally
            {
                await client.DisconnectAsync(true);
                client.Dispose();
            }

            }
    }
}
