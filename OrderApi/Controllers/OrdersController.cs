using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using OrderApi.Data;
using OrderApi.Models;
using RestSharp;

namespace OrderApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IRepository<Order> repository;

        public OrdersController(IRepository<Order> repos)
        {
            repository = repos;
        }

        // GET: orders
        [HttpGet]
        public IEnumerable<Order> Get()
        {
            return repository.GetAll();
        }

        // GET orders/5
        [HttpGet("{id}", Name = "GetOrder")]
        public IActionResult Get(int id)
        {
            var item = repository.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // POST orders
        [HttpPost]
        public IActionResult Post([FromBody] Order order)
        {
            if (order == null)
            {
                return BadRequest();
            }

            // Call ProductApi to get the product ordered
            RestClient c = new RestClient();
            // You may need to change the port number in the BaseUrl below
            // before you can run the request.
            c.BaseUrl = new Uri("http://customerapi/customers/");
            var customer = GetData<Customer>(c, Method.GET, order.CustomerId);

            c.BaseUrl = new Uri("http://productapi/products/");
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
                            // Example of using the email sender class
                            // EmailSender.SendTo(customer.Email);

                            var newOrder = repository.Add(order);
                            return CreatedAtRoute("GetOrder", new { id = newOrder.Id }, newOrder);
                        }
                    } else {
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