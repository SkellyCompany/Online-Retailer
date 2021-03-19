using System;
using System.Threading.Tasks;
using OnlineRetailer.Entities;
using OnlineRetailer.Messaging;
using OnlineRetailer.ProductAPI.Core.DomainServices;

namespace OnlineRetailer.ProductAPI.Core.MessagingReceivers.Receivers
{
    public class DeliveredOrderReceiver : IDeliveredOrderReceiver
    {
        private IMessagingService _messagingService;
        private IProductRepository _productRepository;

        public DeliveredOrderReceiver(IMessagingService messagingService, IProductRepository productRepository)
        {
            _messagingService = messagingService;
            _productRepository = productRepository;
        }

        public void Start()
        {
            Task.Factory.StartNew(() =>
            {
                _messagingService.Receive("deliveredOrder", (result) =>
                    {
                        if (result is Order)
                        {
                            Order order = result as Order;
                            foreach (OrderLine line in order.OrderLines)
                            {
                                Product product = _productRepository.Get(line.ProductId);
                                if (product != null)
                                {
                                    product.ItemsReserved -= line.Quantity;
                                    product.ItemsInStock -= line.Quantity;
                                    _productRepository.Edit(product);
                                }
                            }
                        }

                    });
            });
        }
    }
}