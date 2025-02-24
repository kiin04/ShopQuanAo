using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMusic.Models;

namespace WebMusic.Controllers
{
    public class DetailController : Controller
    {
        ShopQuanAoEntities db = new ShopQuanAoEntities();
        // GET: Detail
        public ActionResult Detail(int id)
        {
            var product = db.Products.FirstOrDefault(p => p.ProductID == id);
            if (product == null)
            {
                return HttpNotFound();
            }
            // Lấy danh sách bình luận
            var comments = db.Comments
                             .Where(c => c.ProductID == id)
                             .OrderByDescending(c => c.CreatedAt)
                             .ToList();

            // Truyền dữ liệu bằng ViewBag
            ViewBag.ProductID = product.ProductID;
            ViewBag.ProductName = product.ProductName;
            ViewBag.CategoryName = product.Category?.CategoryName;
            ViewBag.Price = product.Price;
            ViewBag.Stock = product.Stock;
            ViewBag.Description = product.Description;
            ViewBag.ImageURL = product.ImageURL;
            ViewBag.Comments = comments;

            return View();
        }

        [HttpPost]
        public JsonResult AddComment(int productId, string commentText)
        {
            if (Session["Email"] == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để bình luận." });
            }

            // Lấy thông tin khách hàng đang đăng nhập từ Session
            string userEmail = Session["Email"].ToString();
            var customer = db.Customers.FirstOrDefault(c => c.Email == userEmail);

            if (customer == null)
            {
                return Json(new { success = false, message = "Tài khoản không hợp lệ." });
            }

            // Tạo bình luận mới
            var comment = new Comment
            {
                ProductID = productId,
                CustomerID = customer.CustomerID,
                CommentText = commentText,
                CreatedAt = DateTime.Now
            };

            db.Comments.Add(comment);
            db.SaveChanges();

            return Json(new
            {
                success = true,
                customerName = customer.FullName,
                createdAt = comment.CreatedAt?.ToString("dd/MM/yyyy HH:mm") // Chuyển thành chuỗi đúng format
            });
        }

    }
}