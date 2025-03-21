using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMusic.Models
{
    public class ProductRepository
    {
        private readonly ShopQuanAoEntities _db;

        public ProductRepository()
        {
            _db = new ShopQuanAoEntities();
        }

        public List<Product> GetAllProducts()
        {
            return _db.Products.ToList();
        }

        public Product GetProductById(int id)
        {
            return _db.Products.Find(id);
        }

        public bool AddProduct(Product product)
        {
            var existingProduct = _db.Products.FirstOrDefault(p => p.ProductName == product.ProductName);
            if (existingProduct != null)
            {
                return false;
            }

            _db.Products.Add(product);
            _db.SaveChanges();
            return true;
        }

        public bool UpdateProduct(Product product)
        {
            var existingProduct = _db.Products.Find(product.ProductID);
            if (existingProduct == null)
            {
                return false;
            }

            existingProduct.ProductName = product.ProductName;
            existingProduct.CategoryID = product.CategoryID;
            existingProduct.Price = product.Price;
            existingProduct.Stock = product.Stock;
            existingProduct.Description = product.Description;
            existingProduct.ImageURL = product.ImageURL;

            _db.SaveChanges();
            return true;
        }

        public bool DeleteProduct(int id)
        {
            var product = _db.Products
                             .FirstOrDefault(p => p.ProductID == id);

            if (product == null)
            {
                return false;
            }

            // Xóa tất cả các đơn hàng liên quan trước (nếu muốn xóa cứng)
            _db.OrderDetails.RemoveRange(product.OrderDetails);

            _db.Products.Remove(product);
            _db.SaveChanges();
            return true;
        }
    }
}