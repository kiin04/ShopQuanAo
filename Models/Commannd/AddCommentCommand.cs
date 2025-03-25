using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMusic.Models.Commannd
{
    public class AddCommentCommand : ICommand
    {
        //DetailController AddComment

        private readonly int _productId;
        private readonly string _commentText;
        private readonly HttpSessionStateBase _session;
        private readonly ShopQuanAoEntities _db;
        private int productId;
        private string commentText;
        private HttpSessionStateBase session;
        private ShopQuanAoEntities db;

        public AddCommentCommand(int productId, string commentText, HttpSessionStateBase session, ShopQuanAoEntities db)
        {
            _productId = productId;
            _commentText = commentText;
            _session = session;
            _db = db;
        }

        public JsonResult Execute()
        {
            if (_session["Email"] == null)
            {
                return new JsonResult { Data = new { success = false, message = "Bạn cần đăng nhập để bình luận." } };
            }

            string userEmail = _session["Email"].ToString();
            var customer = _db.Customers.FirstOrDefault(c => c.Email == userEmail);

            if (customer == null)
            {
                return new JsonResult { Data = new { success = false, message = "Tài khoản không hợp lệ." } };
            }

            var comment = new Comment
            {
                ProductID = _productId,
                CustomerID = customer.CustomerID,
                CommentText = _commentText,
                CreatedAt = DateTime.Now
            };

            _db.Comments.Add(comment);
            _db.SaveChanges();

            return new JsonResult
            {
                Data = new
                {
                    success = true,
                    customerName = customer.FullName,
                    createdAt = comment.CreatedAt?.ToString("dd/MM/yyyy HH:mm")
                }
            };
        }
    }
}