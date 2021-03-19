using System;
using System.Threading.Tasks;
using OnlineRetailer.Entities;
using OnlineRetailer.Messaging;
using OnlineRetailer.ProductAPI.Core.DomainServices;

namespace OnlineRetailer.ProductAPI.Core.MessagingReceivers.Receivers
{
    public class NewOrderReceiver : INewOrderReceiver
    {
        private IMessagingService _messagingService;
        private IProductRepository _productRepository;

        public NewOrderReceiver(IMessagingService messagingService, IProductRepository productRepository)
        {
            _messagingService = messagingService;
            _productRepository = productRepository;
        }

        public void Start()
        {
            Task.Factory.StartNew(() =>
            {

                Console.WriteLine("dupka");
                _messagingService.Receive("newOrder", (result) =>
                    {
                        Console.WriteLine("kupka");
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

                    });
            });
        }
    }
}