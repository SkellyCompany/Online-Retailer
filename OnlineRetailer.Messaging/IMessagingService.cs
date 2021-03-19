using System;

namespace OnlineRetailer.Messaging
{
    public interface IMessagingService
    {
        void Publish<T>(string topic, T message);

        void Subscribe<T>(string subscriberId, string topic, Action<T> completion);

        void Send<T>(string queue, T message);

        void Receive<T>(string queue, Action<T> completion);
    }
}
