using System;
using System.Net;
using System.Net.Mail;

namespace OnlineRetailer.OrderApi.Services.Email
{
    public class EmailService : IEmailService
    {

        private readonly SmtpClient _smtpClient;
        private readonly string _senderEmail;
        public EmailService(IEmailSettings emailSettings)
        {
            _smtpClient = new SmtpClient(emailSettings.SmtpServer);
            _smtpClient.Port = emailSettings.SmtpServerPort;
            _smtpClient.Credentials = new NetworkCredential(emailSettings.SenderEmail, emailSettings.SenderEmailPassword);
            _smtpClient.EnableSsl = true;
            _senderEmail = emailSettings.SenderEmail;
        }

        public void Send(string receiverEmail, string subject, string message)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress(_senderEmail);
                mailMessage.To.Add(receiverEmail);
                mailMessage.Subject = subject;
                mailMessage.Body = message;
                _smtpClient.Send(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
