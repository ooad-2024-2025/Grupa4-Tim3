using MailKit.Net.Smtp;
using MimeKit;
using Microsoft.Extensions.Configuration;

namespace MEDIPLAN.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress(
            _configuration["EmailSettings:SenderName"] ?? string.Empty,
            _configuration["EmailSettings:SenderEmail"] ?? string.Empty));

        emailMessage.To.Add(new MailboxAddress("", email));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart("html") { Text = message };

        using var client = new SmtpClient();

        var mailPort = _configuration["EmailSettings:MailPort"];
        if (string.IsNullOrEmpty(mailPort) || !int.TryParse(mailPort, out var parsedPort))
        {
            throw new InvalidOperationException("Invalid or missing MailPort configuration.");
        }

        await client.ConnectAsync(
            _configuration["EmailSettings:MailServer"] ?? throw new InvalidOperationException("MailServer configuration is missing."),
            parsedPort,
            MailKit.Security.SecureSocketOptions.StartTls);

        await client.AuthenticateAsync(
            _configuration["EmailSettings:Username"] ?? throw new InvalidOperationException("Username configuration is missing."),
            _configuration["EmailSettings:Password"] ?? throw new InvalidOperationException("Password configuration is missing."));

        await client.SendAsync(emailMessage);
        await client.DisconnectAsync(true);
    }

    public async Task SendEmailWithAttachmentAsync(string email, string subject, string message, byte[] attachmentBytes, string attachmentName)
    {
        var emailMessage = new MimeMessage();

        emailMessage.From.Add(new MailboxAddress(
            _configuration["EmailSettings:SenderName"] ?? string.Empty,
            _configuration["EmailSettings:SenderEmail"] ?? string.Empty));

        emailMessage.To.Add(new MailboxAddress("", email));
        emailMessage.Subject = subject;

        var builder = new BodyBuilder
        {
            HtmlBody = message
        };

        builder.Attachments.Add(attachmentName, attachmentBytes);

        emailMessage.Body = builder.ToMessageBody();

        using var client = new SmtpClient();

        var mailPort = _configuration["EmailSettings:MailPort"];
        if (string.IsNullOrEmpty(mailPort) || !int.TryParse(mailPort, out var parsedPort))
        {
            throw new InvalidOperationException("Invalid or missing MailPort configuration.");
        }

        await client.ConnectAsync(
            _configuration["EmailSettings:MailServer"] ?? throw new InvalidOperationException("MailServer configuration is missing."),
            parsedPort,
            MailKit.Security.SecureSocketOptions.StartTls);

        await client.AuthenticateAsync(
            _configuration["EmailSettings:Username"] ?? throw new InvalidOperationException("Username configuration is missing."),
            _configuration["EmailSettings:Password"] ?? throw new InvalidOperationException("Password configuration is missing."));

        await client.SendAsync(emailMessage);
        await client.DisconnectAsync(true);
    }
}