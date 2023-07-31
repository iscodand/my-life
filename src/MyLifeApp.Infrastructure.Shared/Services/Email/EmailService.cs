using System.Net;
using System.Net.Mail;

namespace MyLifeApp.Infrastructure.Shared.Services.Email
{
    public class EmailService : IEmailService
    {
        public async Task SendMailAsync(SendMailRequest request)
        {
            MailMessage mailMessage = new();
            SmtpClient smtp = new();
            EmailSettings settings = new();

            try
            {
                // SMTP Settings
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(settings.Email, settings.Password);
                smtp.Port = settings.Port;
                smtp.EnableSsl = true;
                smtp.Host = settings.Host;

                // Create Message
                mailMessage.From = new MailAddress(settings.Email);
                mailMessage.To.Add(request.To);
                mailMessage.Subject = request.Subject;
                mailMessage.Body = request.Body;
                mailMessage.IsBodyHtml = true;

                // SendMail
                await smtp.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}