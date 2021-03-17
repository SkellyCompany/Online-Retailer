using System;

namespace OnlineRetailer.Messaging
{
    public interface IMessagingService
    {
        void Publish(string message, string topic);

        void Subscribe(string subscriberId, string topic, Action<object> completion);
    }
}