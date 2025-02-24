using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebMusic.Models;

namespace WebMusic.Controllers
{
    public class LoginController : Controller
    {
        ShopQuanAoEntities database = new ShopQuanAoEntities();
        // GET: Login
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(string Email, string Password)
        {
            // Kiểm tra trong bảng Customer
            var customer = database.Customers.FirstOrDefault(c => c.Email == Email && c.PasswordHash == Password);
            if (customer != null)
            {
                Session["Avatar"] = customer.Avatar ?? "/Content/images/profile1.png";
                Session["Email"] = customer.Email;
                Session["CurrentUserId"] = customer.CustomerID;
                Session["FullName"] = customer.FullName;
                Session["Role"] = "Customer"; // Gán vai trò khách hàng
                Session["Phone"] = customer.Phone;
                Session["Address"] = customer.Address;
                return RedirectToAction("Home", "Home");
            }

            // Kiểm tra trong bảng User (Admin)
            var admin = database.Users.FirstOrDefault(u => u.Email == Email && u.PasswordHash == Password);
            if (admin != null)
            {
                Session["Email"] = admin.Email;
                Session["CurrentUserId"] = admin.UserID;
                Session["Username"] = admin.Username;
                Session["Role"] = "Admin"; // Gán vai trò admin
                return RedirectToAction("Home", "Home"); // Chuyển hướng đến trang admin
            }

            ViewBag.error = "Tên đăng nhập hoặc mật khẩu không đúng";
            return View();
        }
        public ActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Register(Customer customer, HttpPostedFileBase Avatar)
        {
            if (!customer.Email.EndsWith("@gmail.com"))
            {
                ModelState.AddModelError("Email", "Email phải có đuôi @gmail.com.");
            }

            var existingEmail = database.Customers.FirstOrDefault(u => u.Email == customer.Email);
            if (existingEmail != null)
            {
                ModelState.AddModelError("Email", "Email đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                string avatarPath = "/Content/images/profile1.png"; // Ảnh mặc định nếu không có avatar

                // Xử lý upload avatar
                if (Avatar != null && Avatar.ContentLength > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(Avatar.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Content/avatars/"), fileName);
                    Avatar.SaveAs(path);

                    avatarPath = "/Content/avatars/" + fileName;
                }

                // Lưu user vào database
                var newCus = new Customer
                {
                    Phone = customer.Phone,
                    Address = customer.Address,
                    FullName = customer.FullName,
                    Email = customer.Email,
                    PasswordHash = customer.PasswordHash, // Nên hash mật khẩu trước khi lưu
                    Avatar = avatarPath, // Lưu đường dẫn avatar vào DB
                    CreatedAt = DateTime.Now,
                };

                database.Customers.Add(newCus);
                database.SaveChanges();
                return RedirectToAction("Login");
            }

            // Lưu danh sách lỗi vào ViewBag
            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
            return View();
        }
        public ActionResult Logout(User user)
        {
            Session.Clear();
            return RedirectToAction("Home", "Home");
        }
    }
}