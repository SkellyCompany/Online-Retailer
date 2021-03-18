using Microsoft.AspNetCore.Mvc;
using OnlineRetailer.CustomerAPI.Core.ApplicationServices;
using OnlineRetailer.CustomerAPI.Entities;
using System;
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
        public IActionResult Get()
        {
			try
			{
                return Ok(_customerService.GetAll());
            }
            catch (Exception e)
			{
                return BadRequest(e.Message);
            }
        }

        // Get Customer By ID
        [HttpGet("{id}", Name = "GetCustomer")]
        public IActionResult Get(int id)
        {
			try
			{
                return Ok(_customerService.Get(id));
            }
            catch (Exception e)
			{
                return BadRequest(e.Message);
            }
        }

        // Create Customer
        [HttpPost]
        public IActionResult Post([FromBody] Customer customer)
        {
			try
            {
                return Ok(_customerService.Add(customer));
            }
			catch (Exception e)
			{
                return BadRequest(e.Message);
            }
        }

        // Update Customer
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Customer customer)
        {
            try
            {
                if (id != customer.Id)
                {
                    return Conflict("Parameter ID does not match Country id");
                }
                return Ok(_customerService.Edit(customer));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // Delete Customer
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                return Ok(_customerService.Remove(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
