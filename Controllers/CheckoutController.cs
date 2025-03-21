using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMusic.Models;

namespace WebMusic.Controllers
{
    public class CheckoutController : Controller
    {
        ShopQuanAoEntities db = new ShopQuanAoEntities();
        // GET: Checkout
        public ActionResult ThanhToan()
        {
            var cart = Session["Cart"] as List<Cart>;
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("DanhMuc", "DanhMuc");
            }
            // Kiểm tra nếu người dùng đã đăng nhập
            var cus = Session["Customer"] as Customer; // Giả sử có lưu user trong session
            if (cus == null) // Nếu Session không có, lấy từ database
            {
                int? customerId = Session["CustomerID"] as int?;
                if (customerId != null)
                {
                    cus = db.Customers.Find(customerId);
                    Session["Customer"] = cus; // Lưu lại session để lần sau không cần truy vấn
                }
            }

            if (cus != null)
            {
                ViewBag.FullName = cus.FullName;
                ViewBag.Email = cus.Email;
                ViewBag.Address = cus.Address;
                ViewBag.PhoneNumber = cus.Phone;
            }
            ViewBag.CartItems = cart;
            ViewBag.TotalAmount = cart.Sum(item => item.Quantity * item.Product.Price);
            return View();
        }
        // Xác nhận đặt hàng
        [HttpPost]
        public ActionResult ConfirmOrder(string FullName, string Address, string PhoneNumber, string PaymentMethod)
        {
            var cart = Session["Cart"] as List<Cart>;
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Cart", "Cart");
            }

            var customer = Session["Customer"] as Customer;
            int? customerId = customer != null ? customer.CustomerID : (int?)null;

            // Tạo đơn hàng mới
            Order newOrder = new Order
            {
                CustomerID = customerId, // Nếu có đăng nhập thì lấy ID khách hàng, nếu không thì null
                OrderDate = DateTime.Now,
                TotalAmount = cart.Sum(item => item.Quantity * item.Product.Price),
                Status = "Chờ xác nhận"
            };

            db.Orders.Add(newOrder);
            db.SaveChanges();

            // Lưu chi tiết đơn hàng và cập nhật Stock sản phẩm
            foreach (var item in cart)
            {
                // Tạo chi tiết đơn hàng
                OrderDetail orderDetail = new OrderDetail
                {
                    OrderID = newOrder.OrderID,
                    ProductID = item.Product.ProductID,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                };
                db.OrderDetails.Add(orderDetail);

                // Giảm số lượng tồn kho của sản phẩm
                var product = db.Products.Find(item.Product.ProductID);
                if (product != null)
                {
                    // Đảm bảo số lượng tồn kho không bị âm
                    if (product.Stock >= item.Quantity)
                    {
                        product.Stock -= item.Quantity;
                    }
                    else
                    {
                        return Content($"Sản phẩm {product.ProductName} không đủ số lượng trong kho!");
                    }
                }
            }

            db.SaveChanges();

            // Nếu khách hàng đã đăng nhập và có giỏ hàng trong DB, xóa giỏ hàng của họ
            if (customerId.HasValue)
            {
                // Tìm và xóa các mặt hàng trong giỏ hàng của khách hàng từ cơ sở dữ liệu
                var cartItemsInDb = db.Carts.Where(c => c.CustomerID == customerId);
                db.Carts.RemoveRange(cartItemsInDb); // Xóa các sản phẩm trong giỏ hàng của khách hàng
                db.SaveChanges();
            }

            // Xóa giỏ hàng trong session
            Session["Cart"] = null;
            Session["CartCount"] = 0;
            return RedirectToAction("OrderSuccess", new { id = newOrder.OrderID });
        }


        // Hiển thị trang đặt hàng thành công
        public ActionResult OrderSuccess(int id)
        {
            var order = db.Orders
                .Include("OrderDetails.Product") // Load chi tiết sản phẩm
                .Include("Customer") // Load thông tin khách hàng
                .FirstOrDefault(o => o.OrderID == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

    }
}