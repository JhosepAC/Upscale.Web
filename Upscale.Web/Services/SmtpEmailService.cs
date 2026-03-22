using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace Upscale.Web.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly EmailSettings _settings;
        private readonly ILogger<SmtpEmailService> _logger;

        public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
        {
            _settings = configuration
                .GetSection("EmailSettings")
                .Get<EmailSettings>()
                ?? throw new InvalidOperationException(
                    "La sección 'EmailSettings' no está configurada en appsettings.json.");

            _logger = logger;
        }

        public async Task SendAsync(string toEmail, string toName, string subject, string htmlBody)
        {
            if (string.IsNullOrWhiteSpace(toEmail))
                throw new ArgumentException("La dirección de destino no puede estar vacía.", nameof(toEmail));

            var message = BuildMessage(toEmail, toName, subject, htmlBody);

            try
            {
                using var client = new SmtpClient();

                await client.ConnectAsync(
                    _settings.Host,
                    _settings.Port,
                    SecureSocketOptions.Auto);

                if (!string.IsNullOrEmpty(_settings.UserName))
                    await client.AuthenticateAsync(_settings.UserName, _settings.Password);

                await client.SendAsync(message);
                await client.DisconnectAsync(quit: true);

                _logger.LogInformation(
                    "Correo '{Subject}' enviado correctamente a {Email}.", subject, toEmail);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error al enviar el correo '{Subject}' a {Email}.", subject, toEmail);
            }
        }

        private MimeMessage BuildMessage(string toEmail, string toName, string subject, string htmlBody)
        {
            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(_settings.SenderName, _settings.SenderEmail));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = htmlBody,
                TextBody = StripHtml(htmlBody)
            };

            message.Body = builder.ToMessageBody();
            return message;
        }

        private static string StripHtml(string html)
        {
            if (string.IsNullOrEmpty(html)) return string.Empty;
            return System.Text.RegularExpressions.Regex
                .Replace(html, "<[^>]+>", " ")
                .Replace("&nbsp;", " ")
                .Replace("&amp;", "&")
                .Replace("&lt;", "<")
                .Replace("&gt;", ">")
                .Replace("&quot;", "\"")
                .Trim();
        }
    }

    public sealed class EmailSettings
    {
        public string Host { get; set; } = string.Empty;
        public int Port { get; set; } = 587;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string SenderEmail { get; set; } = string.Empty;
        public string SenderName { get; set; } = "CEPLAN";
    }
}