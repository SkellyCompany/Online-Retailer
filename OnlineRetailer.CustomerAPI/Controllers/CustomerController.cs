using Microsoft.AspNetCore.Mvc;
using OnlineRetailer.CustomerAPI.Core.ApplicationServices;
using OnlineRetailer.CustomerAPI.Entities;
using System.Collections.Generic;

namespace OnlineRetailer.CustomerAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _customerService;


        public CustomerController(ICustomerService customerService)
        {
            _customerService = customerService;
        }

        // Get All Customers
        [HttpGet]
        public IEnumerable<Customer> Get()
        {
            return _customerService.GetAll();
        }

        // Get Customer By ID
        [HttpGet("{id}", Name = "GetCustomer")]
        public IActionResult Get(int id)
        {
            var item = _customerService.Get(id);
            if (item == null)
            {
                return NotFound($"Could not find Customer with ID: {id}");
            }
            return Ok(item);
        }

        // Create Customer
        [HttpPost]
        public IActionResult Post([FromBody] Customer customer)
        {
            if (customer == null)
            {
                return BadRequest("Could not create customer - customer is null");
            }
            var newCustomer = _customerService.Add(customer);
            return CreatedAtRoute("GetCustomer", new { id = newCustomer.Id }, newCustomer);
        }

        // Update Customer
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Customer customer)
        {
            if (customer == null)
            {
                return BadRequest("Could not update customer - customer is null");
            }
            else if (customer.Id != id)
            {
                return BadRequest("Could not update customer - customer is null");
            }

            var modifiedCustomer = _customerService.Get(id);
            if (modifiedCustomer == null)
            {
                return NotFound($"Could not find Customer with ID: {id}");
            }
            modifiedCustomer.Name = customer.Name;
            modifiedCustomer.Email = customer.Email;
            modifiedCustomer.Phone = customer.Phone;
            modifiedCustomer.BillingAddress = customer.BillingAddress;
            modifiedCustomer.ShippingAddress = customer.ShippingAddress;
            modifiedCustomer.CreditStanding = customer.CreditStanding;

            _customerService.Edit(modifiedCustomer);
            return new NoContentResult();
        }

        // Delete Customer
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (_customerService.Get(id) == null)
            {
                return NotFound($"Could not find Customer with ID: {id}");
            }
            _customerService.Remove(id);
            return new NoContentResult();
        }
    }
}
