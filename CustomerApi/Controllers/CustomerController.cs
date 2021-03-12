using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using OnlineRetailer.CustomerApi.Infrastructure;
using OnlineRetailer.Entities;

namespace OnlineRetailer.CustomerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CustomerController : ControllerBase
    {
        private readonly IRepository<Customer> _repository;

        public CustomerController(IRepository<Customer> repository)
        {
            _repository = repository;
        }

        // Get All Customer
        [HttpGet]
        public IEnumerable<Customer> Get()
        {
            return _repository.GetAll();
        }

        // Get Customer By ID
        [HttpGet("{id}", Name = "GetCustomer")]
        public IActionResult Get(int id)
        {
            var item = _repository.Get(id);
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
            var newCustomer = _repository.Add(customer);
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

            var modifiedCustomer = _repository.Get(id);
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

            _repository.Edit(modifiedCustomer);
            return new NoContentResult();
        }

        // Delete Customer
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (_repository.Get(id) == null)
            {
                return NotFound($"Could not find Customer with ID: {id}");
            }
            _repository.Remove(id);
            return new NoContentResult();
        }
    }
}
