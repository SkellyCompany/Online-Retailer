using Microsoft.AspNetCore.Mvc;
using OnlineRetailer.ProductAPI.Entities;
using System.Collections.Generic;
using OnlineRetailer.ProductAPI.Core.ApplicationServices;

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
        public IEnumerable<Product> Get()
        {
            return _productService.GetAll();
        }

        // Get Product By ID
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var item = _productService.Get(id);
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

            var newProduct = _productService.Add(product);

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

            var modifiedProduct = _productService.Get(id);

            if (modifiedProduct == null)
            {
                return NotFound();
            }

            modifiedProduct.Name = product.Name;
            modifiedProduct.Price = product.Price;
            modifiedProduct.ItemsInStock = product.ItemsInStock;
            modifiedProduct.ItemsReserved = product.ItemsReserved;

            _productService.Edit(modifiedProduct);
            return new NoContentResult();
        }

        // Delete Product
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (_productService.Get(id) == null)
            {
                return NotFound();
            }

            _productService.Remove(id);
            return new NoContentResult();
        }
    }
}
