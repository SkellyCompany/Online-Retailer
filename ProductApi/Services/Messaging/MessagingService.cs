using System;
using System.Diagnostics;
using System.Threading;
using EasyNetQ;

namespace OnlineRetailer.ProductApi.Services.Messaging
{
    public class MessagingService : IMessagingService, IDisposable
    {
        IBus bus;

        public MessagingService()
        {
            bus = RabbitHutch.CreateBus("host=hawk.rmq.cloudamqp.com;virtualHost=qsqurewb;username=qsqurewb;password=UyeOEGtcb6zNFOvv_c3Pi-tZoEHJHgVb");
        }

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