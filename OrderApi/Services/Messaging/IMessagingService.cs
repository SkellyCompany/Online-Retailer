using System;

namespace OnlineRetailer.OrderApi.Services.Messaging
{
    public interface IMessagingService
    {
        void PublishMessage(string message, string topic);

        void Subscribe(string subscriberId, string topic, Action<object> completion);
    }
}