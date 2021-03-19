namespace OnlineRetailer.OrderAPI.Core.ApplicationServices
{
    public interface IEmailService
    {
        void Send(string receiverEmail, string subject, string message);
    }
}
