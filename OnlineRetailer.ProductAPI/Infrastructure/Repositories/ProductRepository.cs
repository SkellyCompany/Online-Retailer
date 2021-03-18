using Microsoft.EntityFrameworkCore;
using OnlineRetailer.ProductAPI.Core.DomainServices;
using OnlineRetailer.ProductAPI.Entities;
using OnlineRetailer.ProductAPI.Infrastructure.Database;
using System.Collections.Generic;
using System.Linq;

namespace OnlineRetailer.ProductAPI.Infrastructure.Repositories
{
	public class ProductRepository : IProductRepository
	{
		private readonly ProductContext _productContext;


		public ProductRepository(ProductContext context)
		{
			_productContext = context;
		}


		public IEnumerable<Product> GetAll()
		{
			return _productContext.Products.AsNoTracking().ToList();
		}

		public Product Get(int id)
		{
			return _productContext.Products.AsNoTracking().FirstOrDefault(p => p.Id == id);
		}

		public Product Add(Product product)
		{
			Product newCustomer = _productContext.Products.Add(product).Entity;
			_productContext.SaveChanges();
			return newCustomer;
		}

		public Product Edit(Product product)
		{
			_productContext.Attach(product).State = EntityState.Modified;
			_productContext.SaveChanges();
			return product;
		}

		public Product Remove(int id)
		{
			Product product = _productContext.Products.FirstOrDefault(p => p.Id == id);
			_productContext.Products.Remove(product);
			_productContext.SaveChanges();
			return product;
		}
	}
}
