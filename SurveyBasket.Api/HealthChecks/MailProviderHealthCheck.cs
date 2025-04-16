using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;
using SurveyBasket.Api.Settings;
using System.Reflection.Metadata.Ecma335;

namespace SurveyBasket.Api.HealthChecks;

public class MailProviderHealthCheck(IOptions<MailSettings> mailSetting) : IHealthCheck
{
    private readonly MailSettings mailSetting = mailSetting.Value;
    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            using var smtp = new SmtpClient();

            smtp.Connect(mailSetting.Host, mailSetting.Port, SecureSocketOptions.StartTls);

            smtp.Authenticate(mailSetting.Mail, mailSetting.Password);
        
           return await Task.FromResult(HealthCheckResult.Healthy());
        }
        catch(Exception ex) 
        {
            return await Task.FromResult(HealthCheckResult.Unhealthy(exception : ex));
        }
    }
}
