using System;
using System.Net;
using System.Net.Mail;

namespace OrderApi
{
	public class EmailSender
	{
		public static void SendTo(string receiverEmail)
		{
			try
			{
				MailMessage mailMessage = new MailMessage();
				SmtpClient SmtpServer = new SmtpClient("smtp.gmail.com");
				mailMessage.From = new MailAddress("skellycompany@gmail.com");
				mailMessage.To.Add(receiverEmail);
				mailMessage.Subject = "Order has shipped - SKELLY";
				mailMessage.Body = "Thank you for your purchase, your order has been shipped.";
				SmtpServer.Port = 587;
				SmtpServer.Credentials = new NetworkCredential("skellycompany@gmail.com", "SKELLYskelly99");
				SmtpServer.EnableSsl = true;
				SmtpServer.Send(mailMessage);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
	}
}
