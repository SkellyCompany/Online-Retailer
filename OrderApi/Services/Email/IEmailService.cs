namespace OnlineRetailer.OrderApi.Services.Email
{
    public interface IEmailService
    {
        void Send(string receiverEmail, string subject, string message);
    }
}
