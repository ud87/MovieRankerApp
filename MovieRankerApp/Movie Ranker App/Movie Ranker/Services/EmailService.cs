using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Movie_Ranker.Services
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration; 
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlContent)
        {
            var smtpHost = Environment.GetEnvironmentVariable("smtpHost");
            if (string.IsNullOrEmpty(smtpHost))
            {
                smtpHost = _configuration["EmailSettings:SmtpHost"];
            }

            var smtpPort = Environment.GetEnvironmentVariable("smtpPort");
            if (string.IsNullOrEmpty(smtpPort))
            {
                smtpPort = _configuration["EmailSettings:SmtpPort"];
            }

            var smtpUsername = Environment.GetEnvironmentVariable("smtpUsername");
            if (string.IsNullOrEmpty(smtpUsername))
            {
                smtpUsername = _configuration["EmailSettings:SmtpUsername"];
            }

            var smtpPassword = Environment.GetEnvironmentVariable("smtpPassword");
            if (string.IsNullOrEmpty(smtpPassword))
            {
                smtpPassword = _configuration["EmailSettings:SmtpPassword"];
            }

            var fromEmail = Environment.GetEnvironmentVariable("fromEmail");
            if (string.IsNullOrEmpty(fromEmail))
            {
                fromEmail = _configuration["EmailSettings:FromEmail"];
            }

            var fromName = Environment.GetEnvironmentVariable("fromName");
            if (string.IsNullOrEmpty(fromName))
            {
                fromName = _configuration["EmailSettings:FromName"];
            }


            using (var client = new SmtpClient(smtpHost, int.Parse(smtpPort)))
            { 
                client.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                client.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = htmlContent,
                    IsBodyHtml = true   //Set this to be true so that the email body is rendered as HTML
                };
                mailMessage.To.Add(toEmail); //Add the recipient email address

                await client.SendMailAsync(mailMessage); //Send the email

            }
        }
    }
}
