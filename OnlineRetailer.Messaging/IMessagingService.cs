using System;

namespace OnlineRetailer.Messaging
{
    public interface IMessagingService
    {
        void Publish(object message, string topic);

        void Subscribe(string subscriberId, string topic, Action<object> completion);

        void Send(string queue, object message);

        void Receive(string queue, Action<object> completion);
    }
}
