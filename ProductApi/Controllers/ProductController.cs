using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using OnlineRetailer.Entities;
using OnlineRetailer.ProductApi.Infrastructure;

namespace OnlineRetailer.CustomerApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IRepository<Product> repository;

        public ProductController(IRepository<Product> repos)
        {
            repository = repos;
        }

        // Get All Products
        [HttpGet]
        public IEnumerable<Product> Get()
        {
            return repository.GetAll();
        }

        // Get Product By ID
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var item = repository.Get(id);
            if (item == null)
            {
                return NotFound();
            }
            return new ObjectResult(item);
        }

        // Create Product
        [HttpPost]
        public IActionResult Post([FromBody] Product product)
        {
            if (product == null)
            {
                return BadRequest();
            }

            var newProduct = repository.Add(product);

            return CreatedAtRoute("GetProduct", new { id = newProduct.Id }, newProduct);
        }

        // Update Product
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Product product)
        {
            if (product == null || product.Id != id)
            {
                return BadRequest();
            }

            var modifiedProduct = repository.Get(id);

            if (modifiedProduct == null)
            {
                return NotFound();
            }

            modifiedProduct.Name = product.Name;
            modifiedProduct.Price = product.Price;
            modifiedProduct.ItemsInStock = product.ItemsInStock;
            modifiedProduct.ItemsReserved = product.ItemsReserved;

            repository.Edit(modifiedProduct);
            return new NoContentResult();
        }

        // Delete Product
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (repository.Get(id) == null)
            {
                return NotFound();
            }

            repository.Remove(id);
            return new NoContentResult();
        }
    }
}
