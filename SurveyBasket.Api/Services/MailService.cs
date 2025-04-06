using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;
using SurveyBasket.Api.Settings;

namespace SurveyBasket.Api.Services;

public class MailService(IOptions<MailSettings> mailSetting ) : IEmailSender
{
    private readonly MailSettings mailSetting = mailSetting.Value;

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var message = new MimeMessage
        {
            Sender = MailboxAddress.Parse(mailSetting.Mail),
            Subject = subject,
        };

        message.To.Add(MailboxAddress.Parse(email));

        var builder = new BodyBuilder
        {
            HtmlBody = htmlMessage
        };

        message.Body = builder.ToMessageBody();

        using var smtp = new SmtpClient();

        smtp.Connect(mailSetting.Host, mailSetting.Port, SecureSocketOptions.StartTls);

        smtp.Authenticate(mailSetting.Mail , mailSetting.Password);
        await smtp.SendAsync(message);
        smtp.Disconnect(true);
    }
}
