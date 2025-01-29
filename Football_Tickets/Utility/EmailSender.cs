
using System.Net.Mail;
using System.Net;
using Football_Tickets.Utility;

namespace Football_Tickets.Utility
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("mr7575436@gmail.com", "vqnc pknm ncor xkvr")
            };

            return client.SendMailAsync(
                new MailMessage(from: "mr7575436@gmail.com",
                                to: email,
                                subject,
                                message
                                )
                {
                    IsBodyHtml = true
                });
        }
    }
}