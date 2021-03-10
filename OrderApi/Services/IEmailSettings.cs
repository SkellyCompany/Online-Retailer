namespace OrderApi.Services
{
    public interface IEmailSettings
    {
        string SmtpServer { get; set; }
        int SmtpServerPort { get; set; }
        string SenderEmail { get; set; }
        string SenderEmailPassword { get; set; }
    }
}