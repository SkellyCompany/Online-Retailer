using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using OnlineRetailer.Entities;
using OnlineRetailer.Messaging;
using OnlineRetailer.ProductAPI.Core.DomainServices;

namespace OnlineRetailer.ProductAPI.Core.Messaging.Receivers
{
    public class NewOrderReceiver : IReceiver
    {
        public void Start(IApplicationBuilder app, IMessagingSettings messagingSettings)
        {
            Task.Factory.StartNew(() =>
            {
                new MessagingService(messagingSettings).Receive("newOrder", (result) =>
                    {
                        using (var scope = app.ApplicationServices.CreateScope())
                        {
                            if (result is Order)
                            {
                                Order order = result as Order;
                                foreach (OrderLine line in order.OrderLines)
                                {
                                    Product product = _productRepository.Get(line.ProductId);
                                    if (product != null)
                                    {
                                        product.ItemsReserved += line.Quantity;
                                        _productRepository.Edit(product);
                                    }
                                }
                            }
                        }
                    });
            });
        }
    }
}