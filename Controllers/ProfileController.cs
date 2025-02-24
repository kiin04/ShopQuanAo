using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMusic.Models;

namespace WebMusic.Controllers
{
    public class ProfileController : Controller
    {
        ShopQuanAoEntities db = new ShopQuanAoEntities();
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
                var admin = db.Users.FirstOrDefault(u => u.UserID == userId);
                if (admin != null)
                {
                    Session["Username"] = admin.Username ?? admin.Email; // Lấy FullName nếu có, nếu không dùng Email
                    return View(admin);
                }
            }
            else
            {
                var customer = db.Customers.FirstOrDefault(c => c.CustomerID == userId);
                if (customer != null)
                {
                    Session["FullName"] = customer.FullName ?? customer.Email; // Lấy FullName nếu có, nếu không dùng Email
                    return View(customer);
                }
            }

            return RedirectToAction("Login", "Login");
        }

    }
}