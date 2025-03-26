using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMusic.Models;
using WebMusic.Models.Abstract_Factory;

namespace WebMusic.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ShopQuanAoEntities _db;

        public ProfileController(ShopQuanAoEntities db)
        {
            _db = db;
        }

        // GET: Profile
        public ActionResult ProfileUser()
        {
            if (Session["CurrentUserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            int userId = (int)Session["CurrentUserId"];
            string role = Session["Role"]?.ToString();

            if (role == "Admin")
            {
                var admin = _db.Users.FirstOrDefault(u => u.UserID == userId);
                if (admin != null)
                {
                    Session["Username"] = admin.Username ?? admin.Email;
                    return View(admin);
                }
            }
            else
            {
                var customer = _db.Customers.FirstOrDefault(c => c.CustomerID == userId);
                if (customer != null)
                {
                    Session["FullName"] = customer.FullName ?? customer.Email;
                    return View(customer);
                }
            }

            return RedirectToAction("Login", "Login");
        }


        public ActionResult OrderHistory()
        {
            if (Session["CurrentUserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            int userId = (int)Session["CurrentUserId"];
            var orders = _db.Orders
                           .Where(o => o.CustomerID == userId)
                           .OrderByDescending(o => o.OrderDate) // Sắp xếp mới nhất lên trên cùng
                           .ToList();

            return View(orders);
        }

        public ActionResult OrderDetails(int orderId)
        {
            if (Session["CurrentUserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            var order = _db.Orders.Include("OrderDetails.Product")
                                 .FirstOrDefault(o => o.OrderID == orderId);

            if (order == null)
            {
                return HttpNotFound();
            }

            return PartialView("OrderDetails", order); // Trả về partial view chứa thông tin chi tiết đơn hàng
        }


        public ActionResult EditProfile()
        {
            if (Session["CurrentUserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            int userId = (int)Session["CurrentUserId"];
            string role = Session["Role"]?.ToString();

            if (role == "Admin")
            {
                var admin = _db.Users.FirstOrDefault(u => u.UserID == userId);
                if (admin != null)
                {
                    var model = new UserViewModel
                    {
                        ID = admin.UserID,
                        FullName = admin.Username,
                        Email = admin.Email,
                        Role = admin.Role
                    };
                    return View(model);
                }
            }
            else
            {
                var customer = _db.Customers.FirstOrDefault(c => c.CustomerID == userId);
                if (customer != null)
                {
                    var model = new UserViewModel
                    {
                        ID = customer.CustomerID,
                        FullName = customer.FullName,
                        Email = customer.Email,
                        Phone = customer.Phone,
                        Address = customer.Address,
                        Avatar = customer.Avatar
                    };
                    return View(model);
                }
            }

            return RedirectToAction("ProfileUser");
        }

        [HttpPost]
        public ActionResult EditProfile(UserViewModel model, HttpPostedFileBase avatarFile)
        {
            if (Session["CurrentUserId"] == null)
            {
                return RedirectToAction("Login", "Login");
            }

            int userId = (int)Session["CurrentUserId"];
            string role = Session["Role"]?.ToString();

            if (role == "Admin")
            {
                var admin = _db.Users.FirstOrDefault(u => u.UserID == userId);
                if (admin != null)
                {
                    admin.Username = model.FullName;
                    admin.Email = model.Email;
                    _db.SaveChanges();
                    Session["Username"] = admin.Username;
                }
            }
            else
            {
                var customer = _db.Customers.FirstOrDefault(c => c.CustomerID == userId);
                if (customer != null)
                {
                    customer.FullName = model.FullName;
                    customer.Email = model.Email;
                    customer.Phone = model.Phone;
                    customer.Address = model.Address;

                    if (avatarFile != null && avatarFile.ContentLength > 0)
                    {
                        string fileName = System.IO.Path.GetFileName(avatarFile.FileName);
                        string path = System.IO.Path.Combine(Server.MapPath("~/Content/avatars/"), fileName);
                        avatarFile.SaveAs(path);
                        customer.Avatar = "/Content/avatars/" + fileName;
                        Session["Avatar"] = customer.Avatar;
                    }

                    _db.SaveChanges();
                    Session["FullName"] = customer.FullName;
                    Session["Phone"] = customer.Phone;
                    Session["Address"] = customer.Address;
                }
            }

            return RedirectToAction("ProfileUser");
        }


    }
}
