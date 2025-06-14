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
            _configuration["EmailSettings:SenderName"],
            _configuration["EmailSettings:SenderEmail"]));

        emailMessage.To.Add(new MailboxAddress("", email));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart("html") { Text = message };

        using var client = new SmtpClient();

        await client.ConnectAsync(
            _configuration["EmailSettings:MailServer"],
            int.Parse(_configuration["EmailSettings:MailPort"]),
            MailKit.Security.SecureSocketOptions.StartTls);


        await client.AuthenticateAsync(
            _configuration["EmailSettings:Username"],
            _configuration["EmailSettings:Password"]);

        await client.SendAsync(emailMessage);
        await client.DisconnectAsync(true);
    }
}