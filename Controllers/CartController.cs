using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMusic.Models;

namespace WebMusic.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        ShopQuanAoEntities db = new ShopQuanAoEntities();

        // Hiển thị giỏ hàng (Đổi từ Index thành Cart)
        private bool IsUserLoggedIn()
        {
            return Session["Email"] != null;
        }
        public ActionResult Cart()
        {
            if (!IsUserLoggedIn())
            {
                return RedirectToAction("Login", "Login");
            }

            string email = Session["Email"].ToString();
            var customer = db.Customers.FirstOrDefault(c => c.Email == email);
            if (customer == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var cartItems = db.Carts.Where(c => c.CustomerID == customer.CustomerID).ToList();
            // Cập nhật số lượng giỏ hàng vào session
            UpdateCartCount(customer.CustomerID);
            Session["Cart"] = cartItems;
            return View(cartItems);
        }

        // Thêm vào giỏ hàng
        [HttpPost]
        public ActionResult AddToCart(int productId, int quantity)
        {
            if (!IsUserLoggedIn())
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để thêm sản phẩm vào giỏ hàng." });
            }

            string email = Session["Email"].ToString();
            var customer = db.Customers.FirstOrDefault(c => c.Email == email);
            if (customer == null)
            {
                return Json(new { success = false, message = "Người dùng không hợp lệ." });
            }

            var cartItem = db.Carts.FirstOrDefault(c => c.CustomerID == customer.CustomerID && c.ProductID == productId);

            if (cartItem != null)
            {
                cartItem.Quantity += quantity;
            }
            else
            {
                db.Carts.Add(new Cart
                {
                    CustomerID = customer.CustomerID,
                    ProductID = productId,
                    Quantity = quantity,
                    AddedAt = DateTime.Now
                });
            }

            db.SaveChanges();
            UpdateCartCount(customer.CustomerID); // Cập nhật số lượng giỏ hàng
            return Json(new { success = true, message = "Sản phẩm đã được thêm vào giỏ hàng." });
        }

        // Cập nhật số lượng sản phẩm trong giỏ hàng
        [HttpPost]
        public ActionResult UpdateCart(int cartId, int quantity)
        {
            if (!IsUserLoggedIn())
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để cập nhật giỏ hàng." });
            }

            var cartItem = db.Carts.FirstOrDefault(c => c.CartID == cartId);
            if (cartItem != null)
            {
                cartItem.Quantity = quantity;
                db.SaveChanges();
                UpdateCartCount(cartItem.CustomerID); // Cập nhật số lượng giỏ hàng
                return Json(new { success = true, message = "Cập nhật thành công." });
            }
            return Json(new { success = false, message = "Không tìm thấy sản phẩm." });
        }

        // Xóa sản phẩm khỏi giỏ hàng
        [HttpPost]
        public ActionResult RemoveFromCart(int cartId)
        {
            if (!IsUserLoggedIn())
            {
                return Json(new { success = false, message = "Bạn cần đăng nhập để xóa sản phẩm khỏi giỏ hàng." });
            }

            var cartItem = db.Carts.Find(cartId);
            if (cartItem != null)
            {
                int customerId = cartItem.CustomerID;
                db.Carts.Remove(cartItem);
                db.SaveChanges();
                UpdateCartCount(customerId); // Cập nhật số lượng giỏ hàng
                return Json(new { success = true, message = "Sản phẩm đã được xóa khỏi giỏ hàng." });
            }
            return Json(new { success = false, message = "Không tìm thấy sản phẩm." });
        }

        public void UpdateCartCount(int customerId)
        {
            var cartCount = db.Carts.Where(c => c.CustomerID == customerId).Sum(c => (int?)c.Quantity) ?? 0;
            Session["CartCount"] = cartCount;
        }

    }
}