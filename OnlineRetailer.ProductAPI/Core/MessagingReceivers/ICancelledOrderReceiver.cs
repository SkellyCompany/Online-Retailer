using OnlineRetailer.Messaging;

namespace OnlineRetailer.ProductAPI.Core.MessagingReceivers
{
    public interface ICancelledOrderReceiver
    {
        void Start();
    }
}