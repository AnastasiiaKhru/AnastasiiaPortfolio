using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AnastasiiaPortfolio.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            
            _smtpServer = _configuration["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
            _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"] ?? "587");
            _smtpUsername = _configuration["EmailSettings:SmtpUsername"] ?? "nastiakhru@gmail.com";
            _smtpPassword = _configuration["EmailSettings:SmtpPassword"] ?? "";
            _fromEmail = _configuration["EmailSettings:FromEmail"] ?? "nastiakhru@gmail.com";

            _logger.LogInformation("EmailService initialized with settings: Server={Server}, Port={Port}, Username={Username}", 
                _smtpServer, _smtpPort, _smtpUsername);

            if (IsPlaceholderOrMissing(_smtpPassword))
            {
                _logger.LogWarning("SMTP Password not set. Add your Gmail App Password to appsettings.Development.json (EmailSettings:SmtpPassword) or run: dotnet user-secrets set \"EmailSettings:SmtpPassword\" \"your-16-char-app-password\"");
            }
        }

        private static bool IsPlaceholderOrMissing(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return true;
            var v = value.Trim();
            return string.Equals(v, "your_password_here", StringComparison.OrdinalIgnoreCase)
                || string.Equals(v, "PASTE_YOUR_GMAIL_APP_PASSWORD_HERE", StringComparison.OrdinalIgnoreCase)
                || string.Equals(v, "your_app_password", StringComparison.OrdinalIgnoreCase);
        }

        public async Task SendContactFormEmailAsync(string recipient, string subject, string name, string email, string message)
        {
            if (IsPlaceholderOrMissing(_smtpPassword))
            {
                _logger.LogError("Cannot send email: SMTP password not configured. Set EmailSettings:SmtpPassword in appsettings.Development.json or User Secrets.");
                throw new InvalidOperationException("SMTP password is not configured. Set EmailSettings:SmtpPassword to a Gmail App Password via appsettings.Development.json or User Secrets.");
            }

            try
            {
                _logger.LogInformation("Attempting to send email. From: {From}, To: {To}, Subject: {Subject}", 
                    _fromEmail, recipient, subject);

                using (var client = new SmtpClient())
                {
                    client.Host = _smtpServer;
                    client.Port = _smtpPort;
                    client.EnableSsl = true;
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    client.Timeout = 30000; // 30 seconds timeout

                    _logger.LogInformation("SMTP client configured. Preparing to send message...");

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_fromEmail, "Portfolio Contact Form"),
                        Subject = subject,
                        Body = $"Name: {name}\nEmail: {email}\n\nMessage:\n{message}",
                        IsBodyHtml = false
                    };
                    mailMessage.To.Add(recipient);

                    _logger.LogInformation("Mail message prepared. Attempting to send...");
                    await client.SendMailAsync(mailMessage);
                    _logger.LogInformation("Email sent successfully!");
                }
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "SMTP error while sending email. StatusCode={StatusCode}, Message={Message}", ex.StatusCode, ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while sending email: {Message}", ex.Message);
                throw;
            }
        }
    }
} 