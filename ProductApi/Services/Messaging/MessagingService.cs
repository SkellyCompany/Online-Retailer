using System;
using System.Threading;
using EasyNetQ;

namespace OnlineRetailer.ProductApi.Services.Messaging
{
    public class MessagingService : IMessagingService, IDisposable
    {
		private readonly IBus _bus;


        public MessagingService()
        {
            _bus = RabbitHutch.CreateBus("host=hawk.rmq.cloudamqp.com;virtualHost=qsqurewb;username=qsqurewb;password=UyeOEGtcb6zNFOvv_c3Pi-tZoEHJHgVb");
        }

        public MessagingService(IMessagingSettings settings)
        {
            _bus = RabbitHutch.CreateBus(settings.ConnectionString);
        }

        public void Dispose()
        {
            _bus.Dispose();
        }

        public void PublishMessage(object message, string topic)
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