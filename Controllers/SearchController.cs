using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMusic.Models;

namespace WebMusic.Controllers
{
    public class SearchController : Controller
    {
        ShopQuanAoEntities db = new ShopQuanAoEntities();
        // GET: Search
        public ActionResult Search(string keyword)
        {
            var products = db.Products
                .Where(p => p.ProductName.Contains(keyword) || p.Description.Contains(keyword))
                .ToList();

            return View(products);
        }

        public JsonResult GetSuggestions(string keyword)
        {
            var suggestions = db.Products
                .Where(p => p.ProductName.Contains(keyword))
                .Select(p => new { p.ProductID, p.ProductName, p.ImageURL })
                .Take(6) // Giới hạn 5 kết quả
                .ToList();

            return Json(suggestions, JsonRequestBehavior.AllowGet);
        }
    }
}