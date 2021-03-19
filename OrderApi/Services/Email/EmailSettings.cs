namespace OnlineRetailer.OrderApi.Services.Email
{
    public class EmailSettings : IEmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpServerPort { get; set; }
        public string SenderEmail { get; set; }
        public string SenderEmailPassword { get; set; }
    }
}