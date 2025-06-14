namespace MEDIPLAN.Services;

public interface IEmailService
{
    Task SendEmailAsync(string email, string subject, string message);
    Task SendEmailWithAttachmentAsync(string email, string subject, string message, byte[] attachmentBytes, string attachmentName);

}