using OnlineRetailer.Messaging;
using Microsoft.AspNetCore.Builder;

namespace OnlineRetailer.ProductAPI.Core.Messaging
{
    public interface IReceiver
    {
        void Start(IApplicationBuilder app, IMessagingSettings messagingSettings);
    }
}