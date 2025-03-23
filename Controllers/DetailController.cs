using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMusic.Models;
using WebMusic.Models.Proxy;

namespace WebMusic.Controllers
{
    public class DetailController : Controller
    {
        ShopQuanAoEntities db = new ShopQuanAoEntities();
        // GET: Detail
        private readonly ProductProxy _productProxy;
        public DetailController()
        {
            _productProxy = new ProductProxy();
        }
        public ActionResult Detail(int id)
        {
            var product = _productProxy.GetProduct(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            var comments = _productProxy.GetComments(id);

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

        [HttpPost]
        public JsonResult ReplyComment(int productId, string commentText, int parentCommentId)
        {
            if (Session["Email"] == null)
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để trả lời bình luận." });
            }

            string userEmail = Session["Email"].ToString();
            var customer = db.Customers.FirstOrDefault(c => c.Email == userEmail);

            if (customer == null)
            {
                return Json(new { success = false, message = "Tài khoản không hợp lệ." });
            }

            var parentComment = db.Comments.FirstOrDefault(c => c.CommentID == parentCommentId);
            if (parentComment == null)
            {
                return Json(new { success = false, message = "Bình luận gốc không tồn tại." });
            }

            var reply = new Comment
            {
                ProductID = productId,
                CustomerID = customer.CustomerID,
                CommentText = commentText,
                CreatedAt = DateTime.Now,
                ParentCommentID = parentCommentId
            };

            try
            {
                db.Comments.Add(reply);
                db.SaveChanges();

                return Json(new
                {
                    success = true,
                    customerName = customer.FullName,
                    createdAt = reply.CreatedAt?.ToString("dd/MM/yyyy HH:mm")
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi lưu bình luận: " + ex.Message });
            }
        }


    }
}