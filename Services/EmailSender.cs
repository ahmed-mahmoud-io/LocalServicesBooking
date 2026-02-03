using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace LocalServicesBooking.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly IConfiguration _configuration;

        public EmailSender(ILogger<EmailSender> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            var host = smtpSettings["Host"] ?? "smtp.gmail.com";
            var port = int.Parse(smtpSettings["Port"] ?? "587");
            var username = smtpSettings["Username"];
            var password = smtpSettings["Password"];
            var fromEmail = smtpSettings["FromEmail"] ?? username;
            var fromName = smtpSettings["FromName"] ?? "LocalServicesBooking";
            var enableSsl = bool.Parse(smtpSettings["EnableSsl"] ?? "true");

            // If SMTP is not configured, just log the email (development mode)
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password) || 
                username == "YOUR_EMAIL@gmail.com" || password == "YOUR_APP_PASSWORD")
            {
                _logger.LogWarning("================ EMAIL NOT CONFIGURED ================");
                _logger.LogWarning("SMTP settings are not configured. Email will not be sent.");
                _logger.LogWarning("To enable email sending, update SmtpSettings in appsettings.json");
                _logger.LogInformation("================ EMAIL CONTENT (DEV MODE) ================");
                _logger.LogInformation($"To: {email}");
                _logger.LogInformation($"Subject: {subject}");
                _logger.LogInformation($"Body: {htmlMessage}");
                _logger.LogInformation("==========================================================");
                return;
            }

            try
            {
                using var client = new SmtpClient(host, port)
                {
                    EnableSsl = enableSsl,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(username, password)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail!, fromName),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent successfully to {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send email to {email}");
                throw;
            }
        }
    }
}
