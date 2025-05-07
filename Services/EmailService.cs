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
            _smtpUsername = _configuration["EmailSettings:SmtpUsername"] ?? "anastasiiakhru@gmail.com";
            _smtpPassword = _configuration["EmailSettings:SmtpPassword"] ?? "";
            _fromEmail = _configuration["EmailSettings:FromEmail"] ?? "anastasiiakhru@gmail.com";

            _logger.LogInformation("EmailService initialized with settings: Server={Server}, Port={Port}, Username={Username}", 
                _smtpServer, _smtpPort, _smtpUsername);
                
            if (string.IsNullOrEmpty(_smtpPassword))
            {
                _logger.LogError("SMTP Password is empty or not configured!");
            }
        }

        public async Task SendContactFormEmailAsync(string recipient, string subject, string name, string email, string message)
        {
            try
            {
                _logger.LogInformation("Attempting to send email. From: {From}, To: {To}, Subject: {Subject}", 
                    _fromEmail, recipient, subject);
                
                _logger.LogInformation("SMTP Settings: Server={Server}, Port={Port}, Username={Username}, SSL=Enabled", 
                    _smtpServer, _smtpPort, _smtpUsername);

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
                _logger.LogError(ex, "SMTP error occurred while sending email. Status Code: {Code}, Message: {Message}, StackTrace: {StackTrace}", 
                    ex.StatusCode, ex.Message, ex.StackTrace);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while sending email: {Message}", ex.Message);
                throw;
            }
        }
    }
} 