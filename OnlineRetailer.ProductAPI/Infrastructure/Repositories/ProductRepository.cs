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

		public Product Add(Product product)
		{
			Product newCustomer = _productContext.Products.Add(product).Entity;
			_productContext.SaveChanges();
			return newCustomer;
		}

		public void Edit(Product product)
		{
			_productContext.Entry(product).State = EntityState.Modified;
			_productContext.SaveChanges();
		}

		public Product Get(int id)
		{
			return _productContext.Products.FirstOrDefault(p => p.Id == id);
		}

		public IEnumerable<Product> GetAll()
		{
			return _productContext.Products.ToList();
		}

		public void Remove(int id)
		{
			Product product = _productContext.Products.FirstOrDefault(p => p.Id == id);
			_productContext.Products.Remove(product);
			_productContext.SaveChanges();
		}
	}
}
