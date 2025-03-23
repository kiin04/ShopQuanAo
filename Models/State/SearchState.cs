using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMusic.Models.State
{
    public class SearchState
    {
        //SearchController

        private readonly ShopQuanAoEntities _db;
        public List<Product> Products { get; private set; }
        public List<object> Suggestions { get; private set; }

        public SearchState(ShopQuanAoEntities db)
        {
            _db = db;
        }

        public void SearchProducts(string keyword)
        {
            Products = _db.Products
                .Where(p => p.ProductName.Contains(keyword) || p.Description.Contains(keyword))
                .ToList();
        }

        public void GetSuggestions(string keyword)
        {
            Suggestions = _db.Products
                .Where(p => p.ProductName.Contains(keyword))
                .Select(p => new { p.ProductID, p.ProductName, p.ImageURL })
                .Take(6)
                .ToList<object>();
        }
    }
}