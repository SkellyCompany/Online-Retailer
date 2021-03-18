using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using OnlineRetailer.Entities;
using OnlineRetailer.OrderApi.Infrastructure;
using OnlineRetailer.OrderApi.Services.Email;
using OnlineRetailer.Messaging;

namespace OnlineRetailer.OrderApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IRepository<Order> _repository;
        private readonly IEmailService _emailService;
        private readonly IMessagingService _messagingService;

        public OrderController(IRepository<Order> repository, IEmailService emailService, IMessagingService messagingService)
        {
            _repository = repository;
            _emailService = emailService;
            _messagingService = messagingService;
        }

        // Get All Orders
        [HttpGet]
        public IEnumerable<Order> Get()
        {
            _messagingService.Publish("ass", "ass");
            return _repository.GetAll();
        }

        // Get Order By ID
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var item = _repository.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // Get All Customer Order
        [HttpGet("customer/{customerId}")]
        public IEnumerable<Order> GetCustomerOrders(int customerId)
        {
            return _repository.GetAll().Where(order => order.CustomerId == customerId);
        }

        // Create Order
        [HttpPost]
        public IActionResult Post([FromBody] Order order)
        {
            if (order == null)
            {
                return BadRequest();
            }

            RestClient c = new RestClient();

            // MARK: Fetching Customer from Customer API
            c.BaseUrl = new Uri("http://localhost:5004/customer/");
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
                        // reduce the number of items in stock for the ordered product,
                        // and create a new order.

                        // TODO: Greg's code
                        //orderedProduct.ItemsReserved += order.Quantity;
                        //var updateRequest = new RestRequest(orderedProduct.Id.ToString(), Method.PUT);
                        //updateRequest.AddJsonBody(orderedProduct);
                        //var updateResponse = c.Execute(updateRequest);

                        //if (updateResponse.IsSuccessful)
                        //{
                            order.Status = OrderStatus.PROCESSED;
                            var newOrder = _repository.Add(order);
                            var productNames = orderedProducts.Select(prod => { return prod.Name; }).ToString();
                            _emailService.Send(customer.Email, $"Order Confirmation {newOrder.Id}", $"Order Confirmation\nOrder Id: {newOrder.Id}\nProduct: {productNames}");
                            return Ok(newOrder);
                        //}
                    }
                    else
                    {
                        // Notify the client on insufficient amount of money
                        return BadRequest(new { Message = "Order rejected: Not enough money! :'(" });
                    }
                }
                else
                {
                    // Notify the client on missing customer
                    return BadRequest(new { Message = "Order rejected: Customer not found!" });
                }
            }
            else
            {
                // Notify the client on lack of products
                return BadRequest(new { Message = "Order rejected: Not enough product!" });
            }

            // If the order could not be created, "return no content".
            return NoContent();
        }

        // Update Order Status
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Order order)
        {
            var modifiedOrder = _repository.Get(id);

            if (modifiedOrder == null)
            {
                return NotFound();
            }

            if (order.Status == OrderStatus.CANCELLED)
            {
                if ((int)modifiedOrder.Status > 1)
                    return BadRequest(new { Message = "Order cannot be canceled!" });
            }

            if (order.Status == OrderStatus.IN_TRANSIT)
            {
                RemovedReservedItems(modifiedOrder);
            }

            modifiedOrder.Status = order.Status;
            _repository.Edit(modifiedOrder);
            return Ok(modifiedOrder);
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

        // Get the customer's current balance
        private decimal CustomerCreditStanding(RestClient c, Customer customer) {
            decimal creditStanding = customer.CreditStanding;
            var customerPurchase = GetCustomerOrders(customer.Id).ToList();

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

        // Remove the items reserved
        private void RemovedReservedItems(Order order)
        {
            foreach (OrderLine orderLine in order.OrderLines)
            {
                // var product = GetProduct "somehow" using orderLine.ProductId
                // product.itemsReserved -= orderLine.Quantity
                // product.itemsOnStock -= orderLine.Quantity
            }
        }
    }
}