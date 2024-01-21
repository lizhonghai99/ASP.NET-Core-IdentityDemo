using Microsoft.AspNetCore.Identity.UI.Services;

namespace AuthCenter.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(ILogger<EmailSender> logger) => _logger = logger;
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogInformation("Sent Email Start....");
            _logger.LogInformation($"Email:{email}");
            _logger.LogInformation($"Subject:{subject}");
            _logger.LogInformation($"Content:{htmlMessage}");
            _logger.LogInformation($"Sent Email End...");
            return Task.CompletedTask;
        }
    }
}
