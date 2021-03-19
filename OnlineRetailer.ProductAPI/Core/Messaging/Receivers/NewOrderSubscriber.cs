using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using OnlineRetailer.Entities;
using OnlineRetailer.Messaging;
using OnlineRetailer.ProductAPI.Core.DomainServices;
using Microsoft.Extensions.DependencyInjection;

namespace OnlineRetailer.ProductAPI.Core.Messaging.Receivers
{
    public class NewOrderSubscriber : ISubscriber
    {
        public void Start(IApplicationBuilder app, IMessagingSettings messagingSettings)
        {
            Task.Factory.StartNew(() =>
            {
                new MessagingService(messagingSettings).Subscribe<Order>("NewOrderSubscriber", "newOrder", (order) =>
                {
                    using (var scope = app.ApplicationServices.CreateScope())
                    {
                        var services = scope.ServiceProvider;
                        var productRepository = services.GetService<IProductRepository>();
                        foreach (OrderLine line in order.OrderLines)
                        {
                            Product product = productRepository.Get(line.ProductId);
                            if (product != null)
                            {
                                product.ItemsReserved += line.Quantity;
                                productRepository.Edit(product);
                            }
                        }
                    }
                });
            });
        }
    }
}