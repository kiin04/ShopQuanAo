using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using WebMusic.Models;

namespace WebMusic.Controllers
{
    public class AdminController : Controller
    {
        ShopQuanAoEntities db = new ShopQuanAoEntities();
        // GET: Admin
        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult ManageProduct()
        {
            var products = db.Products.ToList();
            return View(products);
        }

        public ActionResult AddProduct()
        {
            ViewBag.Categories = new SelectList(db.Categories, "CategoryID", "CategoryName");
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(Product product, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = db.Products.FirstOrDefault(p => p.ProductName == product.ProductName);
                if (existingProduct != null)
                {
                    ViewBag.Error = "Tên sản phẩm đã tồn tại";
                }
                else
                {
                    // Kiểm tra nếu file không null và là ảnh
                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        string fileName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(imageFile.FileName);
                        string path = System.IO.Path.Combine(Server.MapPath("/Content/products/"), fileName);
                        imageFile.SaveAs(path);

                        var newProduct = new Product
                        {
                            ProductName = product.ProductName,
                            CategoryID = product.CategoryID,
                            Price = product.Price,
                            Stock = product.Stock,
                            Description = product.Description,
                            ImageURL = "/Content/products/" + fileName,
                            CreatedAt = DateTime.Now
                        };
                        db.Products.Add(newProduct);
                        db.SaveChanges();
                        return RedirectToAction("ManageProduct");
                    }
                    else
                    {
                        // Nếu không có ảnh hoặc không hợp lệ, hiển thị thông báo lỗi
                        ViewBag.Error = "Vui lòng chọn một file ảnh hợp lệ.";
                    }
                }
            }

            // Truyền lại danh sách thể loại nếu có lỗi
            ViewBag.categories = new SelectList(db.Categories.ToList(), "CategoryID", "CategoryName");
            return View();
        }

        public ActionResult EditProduct(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            // Lấy danh sách danh mục để hiển thị trong dropdown
            ViewBag.Categories = new SelectList(db.Categories, "CategoryID", "CategoryName", product.CategoryID);
            return View(product);
        }

        // POST: Edit Product
        [HttpPost]
        public ActionResult EditProduct(Product model, HttpPostedFileBase ImageUpload)
        {
            if (ModelState.IsValid)
            {
                var existingProduct = db.Products.Find(model.ProductID);
                if (existingProduct != null)
                {
                    existingProduct.ProductName = model.ProductName;
                    existingProduct.CategoryID = model.CategoryID;
                    existingProduct.Price = model.Price;
                    existingProduct.Stock = model.Stock;
                    existingProduct.Description = model.Description;

                    // Xử lý upload ảnh mới nếu có
                    if (ImageUpload != null && ImageUpload.ContentLength > 0)
                    {
                        try
                        {
                            string fileName = Path.GetFileName(ImageUpload.FileName);
                            string path = Path.Combine(Server.MapPath("~/Content/products/"), fileName);

                            // Kiểm tra nếu file đã tồn tại -> đổi tên tránh ghi đè
                            if (System.IO.File.Exists(path))
                            {
                                string fileExt = Path.GetExtension(fileName);
                                string fileWithoutExt = Path.GetFileNameWithoutExtension(fileName);
                                fileName = $"{fileWithoutExt}_{DateTime.Now.Ticks}{fileExt}";
                                path = Path.Combine(Server.MapPath("~/Content/products/"), fileName);
                            }

                            // Lưu file lên server
                            ImageUpload.SaveAs(path);

                            // Cập nhật URL hình ảnh trong database
                            existingProduct.ImageURL = "~/Content/products/" + fileName;
                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "Lỗi khi tải ảnh lên: " + ex.Message);
                        }
                    }

                    db.SaveChanges();
                    TempData["SuccessMessage"] = "Sản phẩm đã được cập nhật thành công!";
                    return RedirectToAction("ManageProduct");
                }
            }

            // Nếu có lỗi, hiển thị lại form với danh mục
            ViewBag.Categories = new SelectList(db.Categories, "CategoryID", "CategoryName", model.CategoryID);
            return View(model);
        }
        [HttpPost]
        public ActionResult DeleteProduct(int id)
        {
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            db.Products.Remove(product);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Sản phẩm đã được xóa thành công!";
            return RedirectToAction("ManageProduct");
        }




        public ActionResult ManageCategory()
        {
            var categories = db.Categories.ToList();
            return View(categories);
        }

        public ActionResult AddCategory()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddCategory(Category category)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra tên thể loại đã tồn tại hay chưa
                var check = db.Categories.FirstOrDefault(c => c.CategoryName == category.CategoryName);
                if (check == null)
                {
                    var newCategory = new Category
                    {
                        CategoryName = category.CategoryName
                    };

                    db.Categories.Add(newCategory);
                    db.SaveChanges();

                    return RedirectToAction("ManageCategory");
                }
                else
                {
                    // Nếu thể loại đã tồn tại, hiển thị thông báo lỗi
                    ViewBag.Error = "Thể loại đã tồn tại.";
                }
            }
            return View(category);
        }
        public ActionResult EditCategory(int id)
            {
                using (var db = new ShopQuanAoEntities())
                {
                    var category = db.Categories.Find(id);
                    if (category == null)
                    {
                        return HttpNotFound();
                    }
                    return View(category);
                }
            }

         [HttpPost]
        public ActionResult EditCategory(Category model)
            {
                if (ModelState.IsValid)
                {
                    using (var db = new ShopQuanAoEntities())
                    {
                        var category = db.Categories.Find(model.CategoryID);
                        if (category != null)
                        {
                            category.CategoryName = model.CategoryName;
                            db.SaveChanges();
                            return RedirectToAction("ManageCategory"); // Chuyển hướng sau khi sửa
                        }
                    }
                }
                return View(model);
            }

            [HttpPost]
            public ActionResult DeleteCategory(int id)
            {
                var category = db.Categories.Find(id);
                if (category == null)
                {
                    return HttpNotFound();
                }

                db.Categories.Remove(category);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Danh mục đã được xóa thành công!";
                return RedirectToAction("ManageCategory");
            }



        public ActionResult ManagerUser()
        {
            var customers = db.Customers.Select(c => new UserViewModel
            {
                ID = c.CustomerID,
                FullName = c.FullName,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address,
                Role = "Customer",
                Avatar = c.Avatar
            }).ToList();

            var admins = db.Users.Select(u => new UserViewModel
            {
                ID = u.UserID,
                FullName = u.Username,
                Email = u.Email,
                Phone = "-",
                Address = "-",
                Role = "Admin",
                Avatar = "/Content/images/7.jpg"
            }).ToList();

            // Gộp danh sách
            var allUsers = customers.Concat(admins).ToList();
            return View(allUsers);
        }



        public ActionResult AddCustomer()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddCustomer(Customer customer, HttpPostedFileBase avatarFile)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra email đã tồn tại chưa
                var check = db.Customers.FirstOrDefault(c => c.Email.Trim() == customer.Email.Trim());
                if (check == null)
                {
                    string avatarPath = "/Content/images/7.jpg"; // Ảnh mặc định

                    // Kiểm tra nếu có tệp ảnh tải lên
                    if (avatarFile != null && avatarFile.ContentLength > 0)
                    {
                        // Đảm bảo thư mục lưu trữ tồn tại
                        string uploadDir = Server.MapPath("~/Content/avatars/");
                        if (!Directory.Exists(uploadDir))
                        {
                            Directory.CreateDirectory(uploadDir);
                        }

                        // Lưu tệp ảnh với tên duy nhất
                        string fileExtension = Path.GetExtension(avatarFile.FileName);
                        string fileName = Guid.NewGuid().ToString() + fileExtension;
                        string filePath = Path.Combine(uploadDir, fileName);
                        avatarFile.SaveAs(filePath);

                        // Lưu đường dẫn vào database (dùng đường dẫn ảo)
                        avatarPath = "/Content/avatars/" + fileName;
                    }

                    var newCus = new Customer
                    {
                        FullName = customer.FullName.Trim(),
                        Email = customer.Email.Trim(),
                        Phone = customer.Phone.Trim(),
                        Address = customer.Address.Trim(),
                        PasswordHash = customer.PasswordHash, // Băm mật khẩu
                        Avatar = avatarPath,
                        CreatedAt = DateTime.Now
                    };

                    db.Customers.Add(newCus);
                    db.SaveChanges();

                    return RedirectToAction("ManagerUser");
                }
                else
                {
                    ViewBag.Error = "Khách hàng đã tồn tại.";
                }
            }
            return View(customer);
        }

        public ActionResult EditCustomer(int id)
        {
            var customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }

            return View(customer);
        }

        [HttpPost]
        public ActionResult EditCustomer(int id, Customer customer, HttpPostedFileBase avatarFile)
        {
            var existingCustomer = db.Customers.Find(id);
            if (existingCustomer == null)
            {
                return HttpNotFound();
            }

            if (ModelState.IsValid)
            {
                // Kiểm tra email không bị trùng với khách hàng khác
                var checkEmail = db.Customers.FirstOrDefault(c => c.Email.Trim() == customer.Email.Trim() && c.CustomerID != id);
                if (checkEmail != null)
                {
                    ViewBag.Error = "Email đã được sử dụng bởi khách hàng khác.";
                    return View(customer);
                }

                // Xử lý ảnh đại diện nếu có file mới
                if (avatarFile != null && avatarFile.ContentLength > 0)
                {
                    string uploadDir = Server.MapPath("~/Content/avatars/");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    string fileExtension = Path.GetExtension(avatarFile.FileName);
                    string fileName = Guid.NewGuid().ToString() + fileExtension;
                    string filePath = Path.Combine(uploadDir, fileName);
                    avatarFile.SaveAs(filePath);

                    existingCustomer.Avatar = "/Content/avatars/" + fileName;
                }

                // Cập nhật thông tin khách hàng
                existingCustomer.FullName = customer.FullName.Trim();
                existingCustomer.Email = customer.Email.Trim();
                existingCustomer.Phone = customer.Phone.Trim();
                existingCustomer.Address = customer.Address.Trim();

                db.SaveChanges();
                return RedirectToAction("ManagerUser");
            }

            return View(customer);
        }

        [HttpPost]
        public ActionResult DeleteCustomer(int id)
        {
            var user = db.Users.FirstOrDefault(u => u.UserID == id);
            var customer = db.Customers.FirstOrDefault(c => c.CustomerID == id);

            // Kiểm tra nếu người dùng là Admin
            if (user != null && user.Role == "Admin")
            {
                int adminCount = db.Users.Count(u => u.Role == "Admin");

                // Nếu chỉ còn 1 Admin duy nhất, không cho phép xóa
                if (adminCount <= 1)
                {
                    TempData["ErrorMessage"] = "Không thể xóa! Cần ít nhất một Admin.";
                    return RedirectToAction("ManagerUser");
                }

                db.Users.Remove(user);
            }
            else if (customer != null) // Nếu là khách hàng thì có thể xóa
            {
                db.Customers.Remove(customer);
            }
            else
            {
                return HttpNotFound();
            }

            db.SaveChanges();
            TempData["SuccessMessage"] = "Người dùng đã được xóa thành công!";
            return RedirectToAction("ManagerUser");
        }

    }
}