namespace OnlineRetailer.Messaging
{
    using System;
    using System.Threading;
    using EasyNetQ;

    public class MessagingService : IMessagingService, IDisposable
    {
        private readonly IBus _bus;


        public MessagingService(IMessagingSettings settings)
        {
            _bus = RabbitHutch.CreateBus(settings.ConnectionString);
        }

        public void Dispose()
        {
            _bus.Dispose();
        }

        public void Publish<T>(string topic, T message)
        {
            _bus.PubSub.Publish(message, topic);
        }

        public void Subscribe<T>(string subscriberId, string topic, Action<T> completion)
        {
            _bus.PubSub.Subscribe(subscriberId, completion, x => x.WithTopic(topic));
            lock (this)
            {
                Monitor.Wait(this);
            }
        }

        public void Send<T>(string queue, T message)
        {
            _bus.SendReceive.Send(queue, message);
        }

        public void Receive<T>(string queue, Action<T> completion)
        {
            _bus.SendReceive.Receive(queue, completion);
            lock (this)
            {
                Monitor.Wait(this);
            }
        }
    }
}
