using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using WebMusic.Models;
using WebMusic.Models.Visitor;

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
            var visitor = new LoginHandler(Session);

            var customer = database.Customers.FirstOrDefault(c => c.Email == Email && c.PasswordHash == Password);
            if (customer != null)
            {
                customer.Accept(visitor);
                return RedirectToAction("Home", "Home");
            }

            var admin = database.Users.FirstOrDefault(u => u.Email == Email && u.PasswordHash == Password);
            if (admin != null)
            {
                admin.Accept(visitor);
                return RedirectToAction("Home", "Home");
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

            if (string.IsNullOrEmpty(customer.PasswordHash) || customer.PasswordHash.Length < 6)
            {
                ModelState.AddModelError("PasswordHash", "Mật khẩu phải có ít nhất 6 ký tự.");
            }

            if (ModelState.IsValid)
            {
                string avatarPath = "/Content/images/7.jpg";

                if (Avatar != null && Avatar.ContentLength > 0)
                {
                    string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(Avatar.FileName);
                    string path = System.IO.Path.Combine(Server.MapPath("~/Content/avatars/"), fileName);
                    Avatar.SaveAs(path);

                    avatarPath = "/Content/avatars/" + fileName;
                }

                var newCus = new Customer
                {
                    Phone = customer.Phone,
                    Address = customer.Address,
                    FullName = customer.FullName,
                    Email = customer.Email,
                    PasswordHash = customer.PasswordHash,
                    Avatar = avatarPath,
                    CreatedAt = DateTime.Now,
                };

                database.Customers.Add(newCus);
                database.SaveChanges();
                return RedirectToAction("Login");
            }

            ViewBag.Errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Home", "Home");
        }

        public ActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgetPassword(string Email)
        {
            var customer = database.Customers.FirstOrDefault(c => c.Email == Email);
            var admin = database.Users.FirstOrDefault(u => u.Email == Email);
            object user = customer ?? (object)admin;

            if (user == null)
            {
                ViewBag.Error = "Email không tồn tại.";
                return View();
            }

            // Lưu Email vào Session để dùng trong bước đặt lại mật khẩu
            Session["ResetEmail"] = Email;
            return RedirectToAction("ResetPassword");
        }

        public ActionResult ResetPassword()
        {
            if (Session["ResetEmail"] == null)
            {
                return RedirectToAction("ForgetPassword");
            }
            return View();
        }

        [HttpPost]
        public ActionResult ResetPassword(string NewPassword, string ConfirmPassword)
        {
            string email = Session["ResetEmail"]?.ToString();
            if (email == null) return RedirectToAction("ForgetPassword");

            // Kiểm tra xem mật khẩu nhập lại có khớp không
            if (NewPassword != ConfirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp.";
                return View();
            }

            if (string.IsNullOrEmpty(NewPassword) || NewPassword.Length < 6)
            {
                ViewBag.Error = "Mật khẩu phải có ít nhất 6 ký tự.";
                return View();
            }

            var customer = database.Customers.FirstOrDefault(c => c.Email == email);
            var admin = database.Users.FirstOrDefault(u => u.Email == email);

            if (customer != null)
            {
                if (customer.PasswordHash == NewPassword) // So sánh trực tiếp (nếu chưa mã hóa)
                {
                    ViewBag.Error = "Mật khẩu mới không được trùng với mật khẩu cũ.";
                    return View();
                }
                customer.PasswordHash = NewPassword; // Lưu trực tiếp mật khẩu không mã hóa
                database.SaveChanges();
                Session.Remove("ResetEmail");
                return RedirectToAction("Login");
            }

            if (admin != null)
            {
                if (admin.PasswordHash == NewPassword) // So sánh trực tiếp (nếu chưa mã hóa)
                {
                    ViewBag.Error = "Mật khẩu mới không được trùng với mật khẩu cũ.";
                    return View();
                }
                admin.PasswordHash = NewPassword; // Lưu trực tiếp mật khẩu không mã hóa
                database.SaveChanges();
                Session.Remove("ResetEmail");
                return RedirectToAction("Login");
            }

            ViewBag.Error = "Có lỗi xảy ra. Vui lòng thử lại.";
            return View();
        }

    }
}