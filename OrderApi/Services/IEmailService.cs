namespace OnlineRetailer.OrderApi.Services
{
    public interface IEmailService
    {
        void Send(string receiverEmail, string subject, string message);
    }
}
