using System;

namespace OnlineRetailer.ProductApi.Services.Messaging
{
    public interface IMessagingService
    {
        void PublishMessage(object message, string topic);

        void Subscribe(string subscriberId, string topic, Action<object> completion);
    }
}