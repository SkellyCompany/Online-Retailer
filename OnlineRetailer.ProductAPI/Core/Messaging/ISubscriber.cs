using OnlineRetailer.Messaging;
using Microsoft.AspNetCore.Builder;

namespace OnlineRetailer.ProductAPI.Core.Messaging
{
    public interface ISubscriber
    {
        void Start(IApplicationBuilder app, IMessagingSettings messagingSettings);
    }
}