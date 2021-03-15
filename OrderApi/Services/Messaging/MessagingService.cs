namespace OnlineRetailer.OrderApi.Services.Messaging
{
    using System;
    using System.Threading;
    using EasyNetQ;

    public class MessagingService : IMessagingService, IDisposable
    {
        IBus _bus;

        public MessagingService(IMessagingSettings settings)
        {
            _bus = RabbitHutch.CreateBus(settings.ConnectionString);
        }

        public void Dispose()
        {
            _bus.Dispose();
        }

        public void PublishMessage(string message, string topic)
        {
            _bus.PubSub.Publish(message, topic);
        }

        public void Subscribe(string subscriberId, string topic, Action<object> completion)
        {
            _bus.PubSub.Subscribe<string>(subscriberId, completion, x => x.WithTopic(topic));
            lock (this)
            {
                Monitor.Wait(this);
            }
        }
    }
}