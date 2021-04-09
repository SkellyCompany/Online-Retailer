using Microsoft.AspNetCore.Mvc;
using OnlineRetailer.Entities;
using OnlineRetailer.Messaging;
using OnlineRetailer.OrderAPI.Core.ApplicationServices;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OnlineRetailer.OrderAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IEmailService _emailService;
        private readonly IMessagingService _messagingService;

        public OrderController(IOrderService orderService, IEmailService emailService, IMessagingService messagingService)
        {
            _orderService = orderService;
            _emailService = emailService;
            _messagingService = messagingService;
        }

        // Get All Orders
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                return Ok(_orderService.GetAll());
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // Get Order By ID
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                return Ok(_orderService.Get(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // Get All Customer Order
        [HttpGet("customer/{customerId}")]
        public IActionResult GetCustomerOrders(int customerId)
        {
            try
            {
                return Ok(_orderService.GetCustomerOrders(customerId));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // Create Order
        [HttpPost]
        public IActionResult Post([FromBody] Order order)
        {
            try
            {
                if (order.OrderLines != null && order.OrderLines.Any())
                {
                    // MARK: Fetching Customer from Customer API
                    RestClient client = new RestClient { BaseUrl = new Uri("http://customerapi/customer/") };
                    var customer = GetData<Customer>(client, Method.GET, order.CustomerId);

                    // MARK: Fetching Product from Product API
                    client.BaseUrl = new Uri("http://productapi/product/");
                    var products = GetData<List<Product>>(client, Method.GET);

                    if (customer is not null && products is not null)
                    {
                        var orderedProducts = products.Where(prod =>
                        {
                            return Array.Exists(order.OrderLines.ToArray(), line => line.ProductId == prod.Id);
                        }).ToList();
                        if (AreProductsAvailable(order, orderedProducts))
                        {

                            decimal creditStanding = CalculateCustomerCreditStanding(customer, products);
                            decimal orderPrice = CalculateOrderPrice(order, orderedProducts);

                            if ((creditStanding - orderPrice) > 0)
                            {
                                order.Status = OrderStatus.PROCESSED;
                                order.Date = DateTime.Now;

                                var newOrder = _orderService.Add(order);
                                var productNames = orderedProducts.Select(prod => { return prod.Name; }).ToString();

                                _emailService.Send(customer.Email, $"Order Confirmation {newOrder.Id}", $"Order Confirmation\nOrder Id: {newOrder.Id}\nProduct: {productNames}");
                                _messagingService.Publish<Order>("newOrder", order);
                                return Ok(newOrder);
                            }
                            else
                            {
                                // Notify the client on insufficient amount of money
                                return BadRequest(new { Message = "Order rejected: Not enough money" });
                            }
                        }
                        else
                        {
                            // Notify the client on lack of products
                            return BadRequest(new { Message = "Order rejected: Not enough products" });
                        }
                    }
                    else if (customer is null)
                    {
                        return BadRequest(new { Message = "Order rejected: Could not find customer" });
                    }
                    else
                    {
                        return BadRequest(new { Message = "Order rejected: Could not find products" });
                    }
                }
                else
                {
                    return BadRequest(new { Message = "Order rejected: Order does not contain any products" });
                }
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }


        }

        // Update Order Status
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Order order)
        {
            try
            {
                return Ok(_orderService.Edit(order));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        [HttpPut("Status/{id}")]
        public IActionResult UpdateStatus([FromRoute] int id, [FromBody] Order order)
        {
            try
            {
                var updatedOrder = _orderService.UpdateStatus(id, order.Status);
                if (order.Status == OrderStatus.DELIVERED)
                {
                    Order fullOrder = _orderService.Get(id);
                    _messagingService.Publish<Order>("deliveredOrder", fullOrder);
                }
                else if (order.Status == OrderStatus.CANCELLED)
                {
                    Order fullOrder = _orderService.Get(id);
                    _messagingService.Publish<Order>("cancelledOrder", fullOrder);
                }
                return Ok(updatedOrder);
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // Use this method to make API calls
        private T GetData<T>(RestClient client, Method methodType, int id)
        {
            var request = new RestRequest(id.ToString(), methodType);
            var response = client.Execute<T>(request);
            return response.Data;
        }

        private T GetData<T>(RestClient client, Method methodType)
        {
            var request = new RestRequest(methodType);
            var response = client.Execute<T>(request);
            return response.Data;
        }

        // Checks if all the product are available
        private bool AreProductsAvailable(Order order, List<Product> orderedProducts)
        {
            foreach (OrderLine orderLine in order.OrderLines)
            {
                var product = orderedProducts.SingleOrDefault(prod => prod.Id == orderLine.ProductId);
                if (product == null || orderLine.Quantity > product.ItemsInStock - product.ItemsReserved)
                {
                    return false;
                }
            }
            return true;
        }

        private decimal CalculateCustomerCreditStanding(Customer customer, List<Product> products)
        {
            decimal creditStanding = customer.CreditStanding;
            var customerOrders = _orderService.GetCustomerOrders(customer.Id).ToList();

            foreach (Order order in customerOrders)
            {
                if (order.OrderLines != null && order.OrderLines.Any())
                {
                    foreach (OrderLine orderLine in order.OrderLines)
                    {
                        var product = products.Find(prod => prod.Id == orderLine.ProductId);
                        if (product != null)
                        {
                            creditStanding -= product.Price * orderLine.Quantity;
                        }

                    }
                }
            }
            return creditStanding;
        }

        // Get the overall price of the order
        private decimal CalculateOrderPrice(Order order, List<Product> orderedProduct)
        {
            decimal price = 0;

            foreach (OrderLine orderLine in order.OrderLines)
            {
                var product = orderedProduct.SingleOrDefault(prod => prod.Id == orderLine.ProductId);
                price += product.Price * orderLine.Quantity;
            }

            return price;
        }
    }
}
