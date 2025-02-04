using Azure.Communication.Email;
using Azure;
using HealthMed.API.AgendamentoConsulta.Models;
using Microsoft.Extensions.Configuration;

namespace HealthMed.API.AgendamentoConsulta.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
    public class MailService(IConfiguration config) : IMailService
    {
        public readonly IConfiguration _config = config;

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            // Copy the connection String Endpoint here
            var client = new EmailClient(_config.GetValue<String>("MailService:ConnectionString"));

            // Fill the EmailMessage

            // Add the Mailfrom Address 
            var sender = _config.GetValue<String>("MailService:MailFromAddress");
            var subject = mailRequest.Subject;
            

            var emailContent = new EmailContent(subject)
            {
                PlainText = mailRequest.Body
            };

            List<EmailAddress> toRecipients = new List<EmailAddress>();
            toRecipients.Add(new(mailRequest.To));
            if (mailRequest.Cc != null)
                toRecipients.Add(new(mailRequest.Cc));

            var emailRecipients = new EmailRecipients(toRecipients);

            var emailMessage = new EmailMessage(sender, emailRecipients, emailContent);

            await client.SendAsync(WaitUntil.Started, emailMessage);
            //await client.SendAsync(WaitUntil.Completed, emailMessage);
        }
    }
}
