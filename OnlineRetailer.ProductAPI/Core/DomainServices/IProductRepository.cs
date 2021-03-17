using OnlineRetailer.ProductAPI.Entities;
using System.Collections.Generic;

namespace OnlineRetailer.ProductAPI.Core.DomainServices
{
	public interface IProductRepository
	{
		IEnumerable<Product> GetAll();
		Product Get(int id);
		Product Add(Product product);
		void Edit(Product product);
		void Remove(int id);
	}
}
