using OnlineRetailer.ProductAPI.Entities;
using System.Collections.Generic;

namespace OnlineRetailer.ProductAPI.Core.DomainServices
{
	public interface IProductRepository
	{
		IEnumerable<Product> GetAll();
		Product Get(int id);
		Product Add(Product product);
		Product Edit(Product product);
		Product Remove(int id);
	}
}
