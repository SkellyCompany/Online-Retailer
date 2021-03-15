using System;
using EasyNetQ;

namespace OnlineRetailer.OrderApi.Services.Messaging
{
    using System;
    using System.Threading;
    using EasyNetQ;

    public class MessagingService : IMessagingService, IDisposable
    {
        IBus bus;

        public MessagingService(IMessagingSettings settings)
        {
            bus = RabbitHutch.CreateBus(settings.ConnectionString);
        }

        public void Dispose()
        {
            bus.Dispose();
        }

        public void PublishMessage(object message, string topic)
        {
            bus.PubSub.Publish(message, topic);
        }

        public void Subscribe(string subscriberId, string topic, Action<object> completion)
        {
            bus.PubSub.Subscribe<String>(subscriberId, completion, x => x.WithTopic(topic));
            lock (this)
            {
                Monitor.Wait(this);
            }
        }
    }
}