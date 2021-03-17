namespace OnlineRetailer.Messaging
{
    using System;
    using System.Threading;
    using EasyNetQ;

    public class MessagingService : IMessagingService, IDisposable
    {
        private readonly IBus _bus;


        public MessagingService()
        {
            _bus = RabbitHutch.CreateBus("host=hawk.rmq.cloudamqp.com;virtualHost=qsqurewb;username=qsqurewb;password=UyeOEGtcb6zNFOvv_c3Pi-tZoEHJHgVb");
        }

        public void Dispose()
        {
            _bus.Dispose();
        }

        public void Publish(object message, string topic)
        {
            _bus.PubSub.Publish(message, topic);
        }

        public void Subscribe(string subscriberId, string topic, Action<object> completion)
        {
            _bus.PubSub.Subscribe(subscriberId, completion, x => x.WithTopic(topic));
            lock (this)
            {
                Monitor.Wait(this);
            }
        }

        public void Send(string queue, object message)
        {
            _bus.SendReceive.Send(queue, message);
        }

        public void Receive(string queue, Action<object> completion)
        {
            _bus.SendReceive.Receive(queue, completion);
            lock (this)
            {
                Monitor.Wait(this);
            }
        }
    }
}
