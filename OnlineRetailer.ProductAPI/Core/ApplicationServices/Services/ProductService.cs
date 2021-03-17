using OnlineRetailer.ProductAPI.Core.DomainServices;
using OnlineRetailer.ProductAPI.Entities;
using System.Collections.Generic;

namespace OnlineRetailer.ProductAPI.Core.ApplicationServices.Services
{
	public class ProductService : IProductService
	{
		private readonly IProductRepository _productRepository;


		public ProductService(IProductRepository productRepository)
		{
			_productRepository = productRepository;
		}

		public Product Add(Product product)
		{
			return _productRepository.Add(product);
		}

		public void Edit(Product product)
		{
			_productRepository.Edit(product);
		}

		public Product Get(int id)
		{
			return _productRepository.Get(id);
		}

		public IEnumerable<Product> GetAll()
		{
			return _productRepository.GetAll();
		}

		public void Remove(int id)
		{
			_productRepository.Remove(id);
		}
	}
}
