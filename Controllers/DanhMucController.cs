using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMusic.Models;

namespace WebMusic.Controllers
{
    public class DanhMucController : Controller
    {
        ShopQuanAoEntities db = new ShopQuanAoEntities();
        // GET: DanhMuc
        public ActionResult DanhMuc(int? categoryId, string sortOrder)
        {
            var products = db.Products.AsQueryable(); // Truy vấn danh sách sản phẩm

            // Lọc theo danh mục nếu có
            if (categoryId.HasValue && categoryId != 0)
            {
                products = products.Where(p => p.CategoryID == categoryId);
            }
            ViewBag.SelectedCategoryId = categoryId; // Lưu danh mục đã chọn

            // Sắp xếp theo giá nếu có
            switch (sortOrder)
            {
                case "price_asc":
                    products = products.OrderBy(p => p.Price);
                    ViewBag.SelectedSort = "price_asc";
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price);
                    ViewBag.SelectedSort = "price_desc";
                    break;
                default:
                    ViewBag.SelectedSort = null;
                    break;
            }

            ViewBag.Categories = db.Categories.ToList(); // Truyền danh mục vào View
            return View(products.ToList()); // Trả danh sách sản phẩm đã lọc
        }



    }
}