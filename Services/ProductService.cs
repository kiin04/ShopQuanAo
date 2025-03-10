using System.Collections.Generic;
using System.Linq;
using WebMusic.Models;
using WebMusic.Repositories;

namespace WebMusic.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;

        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public IEnumerable<Product> GetAllProducts() => _productRepository.GetAll();

        public Product GetProductById(int id) => _productRepository.GetById(id);

        public bool AddProduct(Product product)
        {
            if (_productRepository.GetAll().Any(p => p.ProductName == product.ProductName))
                return false; // Tránh trùng tên sản phẩm

            _productRepository.Add(product);
            _productRepository.Save();
            return true;
        }

        public bool UpdateProduct(Product product)
        {
            if (_productRepository.GetById(product.ProductID) == null)
                return false;

            _productRepository.Update(product);
            _productRepository.Save();
            return true;
        }

        public bool DeleteProduct(int id)
        {
            if (_productRepository.GetById(id) == null)
                return false;

            _productRepository.Delete(id);
            _productRepository.Save();
            return true;
        }
    }
}
