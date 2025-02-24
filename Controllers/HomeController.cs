using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMusic.Models;

namespace WebMusic.Controllers
{
    public class HomeController : Controller
    {
        ShopQuanAoEntities db = new ShopQuanAoEntities();

        public ActionResult Home()
        {
            var categories = db.Categories.ToList(); // Lấy danh mục từ database
            var products = db.Products.Take(5).ToList(); // Lấy 5 sản phẩm bất kỳ

            // Truyền dữ liệu qua ViewBag
            ViewBag.Categories = categories;
            ViewBag.Products = products;

            return View();
        }

    }
}