using System;
using System.Net;
using System.Net.Mail;

namespace OnlineRetailer.OrderAPI.Core.ApplicationServices.Services
{
    public class EmailService : IEmailService
    {
        private readonly SmtpClient _smtpClient;
        private readonly string _senderEmail;


        public EmailService()
        {
			_smtpClient = new SmtpClient("smtp.gmail.com")
			{
				Port = 587,
				Credentials = new NetworkCredential("skellycompany@gmail.com", "SKELLYskelly99"),
				EnableSsl = true
			};
			_senderEmail = "skellycompany@gmail.com";
        }

        public void Send(string receiverEmail, string subject, string message)
        {
            try
            {
				MailMessage mailMessage = new MailMessage
				{
					From = new MailAddress(_senderEmail)
				};
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
