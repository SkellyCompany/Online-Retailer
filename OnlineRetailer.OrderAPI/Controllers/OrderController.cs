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
                return Ok(_orderService.Add(order));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }

            // MARK: Fetching Customer from Customer API
            RestClient c = new RestClient { BaseUrl = new Uri("http://localhost:5004/customer/") };
			var customer = GetData<Customer>(c, Method.GET, order.CustomerId);
            // MARK: Fetching Product from Product API
            c.BaseUrl = new Uri("http://localhost:5000/product/");
            var orderedProducts = GetProducts(c, Method.GET, order);

            if (IsProductAvailable(order, orderedProducts))
            {
                if (customer != null)
                {
                    decimal creditStanding = CustomerCreditStanding(c, customer);
                    decimal orderPrice = GetOrderPrice(order, orderedProducts);
                    if ((creditStanding - orderPrice) > 0)
                    {
                        order.Status = OrderStatus.PROCESSED;
                        var newOrder = _orderService.Add(order);
                        var productNames = orderedProducts.Select(prod => { return prod.Name; }).ToString();
                        _emailService.Send(customer.Email, $"Order Confirmation {newOrder.Id}", $"Order Confirmation\nOrder Id: {newOrder.Id}\nProduct: {productNames}");
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
                    // Notify the client on missing customer
                    return BadRequest(new { Message = "Order rejected: Customer not found" });
                }
            }
            else
            {
                // Notify the client on lack of products
                return BadRequest(new { Message = "Order rejected: Not enough products" });
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

        // Use this method to make API calls
        private T GetData<T>(RestClient c, Method methodType, int id)
        {
            var request = new RestRequest(id.ToString(), methodType);
            var response = c.Execute<T>(request);
            return response.Data;
        }

        // Gets the products from the ProductApi
        private List<Product> GetProducts(RestClient c, Method methodType, Order order)
        {
            var ids = order.OrderLines.Select(ol => { return ol.ProductId; }).ToArray();

            var responseData = new List<Product>();
            foreach (int id in ids)
            {
                var request = new RestRequest(id.ToString(), methodType);
                var response = c.Execute<Product>(request);
                responseData.Add(response.Data);
            }
            return responseData;
        }

        // Checks if all the product are available
        private bool IsProductAvailable(Order order, List<Product> orderedProducts)
        {
            foreach (OrderLine orderLine in order.OrderLines)
            {
                var product = orderedProducts.SingleOrDefault(prod => prod.Id == orderLine.ProductId);
                if (orderLine.Quantity > product.ItemsInStock - product.ItemsReserved)
                    return false;
            }
            return true;
        }

        private decimal CustomerCreditStanding(RestClient c, Customer customer)
        {
            decimal creditStanding = customer.CreditStanding;
            var customerPurchase = _orderService.GetCustomerOrders(customer.Id).ToList();

            foreach (Order order in customerPurchase)
            {
                if (order.OrderLines != null && order.OrderLines.Any())
                {
                    foreach (OrderLine orderLine in order.OrderLines)
                    {
                        var product = GetData<Product>(c, Method.GET, orderLine.ProductId);
                        var orderPrice = product.Price * orderLine.Quantity;
                        creditStanding -= orderPrice;
                    }
                }
            }

            return creditStanding;
        }

        // Get the overall price of the order
        private decimal GetOrderPrice(Order order, List<Product> orderedProduct)
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
