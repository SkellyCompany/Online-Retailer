using Microsoft.AspNetCore.Mvc;
using OnlineRetailer.ProductAPI.Core.ApplicationServices;
using System;
using OnlineRetailer.Entities;

namespace OnlineRetailer.ProductAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;


        public ProductController(IProductService productService)
        {
            this._productService = productService;
        }

        // Get All Products
        [HttpGet]
        public IActionResult Get()
        {
			try
			{
                return Ok(_productService.GetAll());
            }
            catch (Exception e)
			{

                return BadRequest(e.Message);
			}
        }

        // Get Product By ID
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                return Ok(_productService.Get(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // Create Product
        [HttpPost]
        public IActionResult Post([FromBody] Product product)
        {
            try
            {
                return Ok(_productService.Add(product));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // Update Product
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Product product)
        {
            try
            {
                return Ok(_productService.Edit(product));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }

        // Delete Product
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                return Ok(_productService.Remove(id));
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
