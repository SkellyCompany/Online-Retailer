using OnlineRetailer.Messaging;

namespace OnlineRetailer.ProductAPI.Core.MessagingReceivers
{
    public interface IDeliveredOrderReceiver
    {
        void Start();
    }
}