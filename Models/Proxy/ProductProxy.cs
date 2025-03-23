using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMusic.Models.Proxy
{
    public class ProductProxy
    {
        //DetailController Detail

        private readonly ShopQuanAoEntities _db;

        public ProductProxy()
        {
            _db = new ShopQuanAoEntities();
        }

        public Product GetProduct(int id)
        {
            return _db.Products.FirstOrDefault(p => p.ProductID == id);
        }

        public List<Comment> GetComments(int productId)
        {
            return _db.Comments
                      .Where(c => c.ProductID == productId)
                      .OrderByDescending(c => c.CreatedAt)
                      .ToList();
        }
    }
}