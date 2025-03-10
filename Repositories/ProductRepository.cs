using System.Collections.Generic;
using System.Linq;
using WebMusic.Models;

namespace WebMusic.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ShopQuanAoEntities _db;

        public ProductRepository(ShopQuanAoEntities db)
        {
            _db = db;
        }

        public IEnumerable<Product> GetAll() => _db.Products.ToList();

        public Product GetById(int id) => _db.Products.Find(id);

        public void Add(Product product)
        {
            _db.Products.Add(product);
        }

        public void Update(Product product)
        {
            var existingProduct = _db.Products.Find(product.ProductID);
            if (existingProduct != null)
            {
                existingProduct.ProductName = product.ProductName;
                existingProduct.CategoryID = product.CategoryID;
                existingProduct.Price = product.Price;
                existingProduct.Stock = product.Stock;
                existingProduct.Description = product.Description;
                existingProduct.ImageURL = product.ImageURL;
            }
        }

        public void Delete(int id)
        {
            var product = _db.Products.Find(id);
            if (product != null)
            {
                _db.Products.Remove(product);
            }
        }

        public void Save() => _db.SaveChanges();
    }
}
