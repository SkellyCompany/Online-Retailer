using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using OnlineRetailer.Entities;
using OnlineRetailer.OrderApi.Services;
using OnlineRetailer.OrderApi.Infrastructure;

namespace OnlineRetailer.OrderApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IRepository<Order> _repository;
        private readonly IEmailService _emailService;

        public OrderController(IRepository<Order> repository, IEmailService emailService)
        {
            _repository = repository;
            _emailService = emailService;
        }

        // Get All Orders
        [HttpGet]
        public IEnumerable<Order> Get()
        {
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
            var orderedProduct = GetData<Product>(c, Method.GET, order.ProductId);

            if (order.Quantity <= orderedProduct.ItemsInStock - orderedProduct.ItemsReserved)
            {
                if (customer != null)
                {
                    if ((customer.CreditStanding - orderedProduct.Price * order.Quantity) > 0)
                    {
                        // reduce the number of items in stock for the ordered product,
                        // and create a new order.
                        orderedProduct.ItemsReserved += order.Quantity;
                        var updateRequest = new RestRequest(orderedProduct.Id.ToString(), Method.PUT);
                        updateRequest.AddJsonBody(orderedProduct);
                        var updateResponse = c.Execute(updateRequest);

                        if (updateResponse.IsSuccessful)
                        {
                            order.Status = OrderStatus.PROCESSED;
                            var newOrder = _repository.Add(order);
                            _emailService.Send(customer.Email, $"Order Confirmation {newOrder.Id}", $"Order Confirmation\nOrder Id: {newOrder.Id}\nProduct: {orderedProduct.Name}\nQuantity: {newOrder.Quantity}");
                            return CreatedAtRoute("GetOrder", new { id = newOrder.Id }, newOrder);
                        }
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

        // Get the customer's current balance
        /*private decimal CustomerCreditStanding(RestClient c, Customer customer) {
            var customerCredit = customer.CreditStanding;
            var customerPurchase = (List<Order>)GetAllOrderForCustomer(customer.Id);
            customerPurchase.ForEach(order => 
            {
                var product = GetData<Product>(c, Method.GET, order.ProductId);
                var orderPrice = product.Price * order.Quantity;
                customerCredit -= orderPrice;
            });
            return customerCredit;
        }*/
    }
}