using OnlineRetailer.ProductAPI.Core.DomainServices;
using OnlineRetailer.ProductAPI.Entities;
using System;
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

		public IEnumerable<Product> GetAll()
		{
			return _productRepository.GetAll();
		}

		public Product Get(int id)
		{
			if (_productRepository.Get(id) == null)
			{
				throw new NullReferenceException($"Could not find Product with ID: {id}");
			}
			return _productRepository.Get(id);
		}

		public Product Add(Product product)
		{
			if (product == null)
			{
				throw new NullReferenceException("Product is null");
			}
			return _productRepository.Add(product);
		}

		public Product Edit(Product product)
		{
			if (product == null)
			{
				throw new NullReferenceException("Product is null");
			}
			if (_productRepository.Get(product.Id) == null)
			{
				throw new NullReferenceException($"Could not find Product with ID: {product.Id}");
			}
			return _productRepository.Edit(product);
		}

		public Product Remove(int id)
		{
			if (_productRepository.Get(id) == null)
			{
				throw new NullReferenceException($"Could not find Product with ID: {id}");
			}
			return _productRepository.Remove(id);
		}
	}
}
