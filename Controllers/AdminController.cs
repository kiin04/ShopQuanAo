using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using WebMusic.Models;
using WebMusic.Models.TemplateMethod;
using WebMusic.Services;

namespace WebMusic.Controllers
{
    public class AdminController : Controller
    {
        private readonly IProductService _productService;
        private readonly ShopQuanAoEntities _db; // Sử dụng ShopQuanAoEntities thay vì ApplicationDbContext

        // Constructor nhận IProductService từ Dependency Injection
        public AdminController(IProductService productService, ShopQuanAoEntities db)
        {
            _productService = productService ?? throw new System.ArgumentNullException(nameof(productService));
            _db = db ?? throw new System.ArgumentNullException(nameof(db));
        }
        // === QUẢN LÝ KHÁCH HÀNG ===

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Dashboard()
        {
            // Thống kê tổng số đơn hàng
            int totalOrders = _db.Orders.Count();

            // Thống kê tổng doanh thu
            decimal totalRevenue = _db.Orders
                .Where(o => o.Status == "Đã giao")
                .Sum(o => (decimal?)o.TotalAmount) ?? 0;

            // Đơn hàng theo trạng thái
            var ordersByStatus = _db.Orders
                .GroupBy(o => o.Status)
                .Select(g => new OrderStatusViewModel
                {
                    Status = g.Key,
                    Count = g.Count()
                }).ToList();

            // Lấy năm hiện tại
            int currentYear = DateTime.Now.Year;

            // Doanh thu theo tháng (đảm bảo đủ 12 tháng)
            var revenueData = _db.Orders
                .Where(o => o.OrderDate.HasValue)
                .GroupBy(o => new { o.OrderDate.Value.Year, o.OrderDate.Value.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Revenue = g.Sum(o => (decimal?)o.TotalAmount) ?? 0
                })
                .ToList();

            // Tạo danh sách đủ 12 tháng với giá trị mặc định là 0
            var fullRevenueByMonth = Enumerable.Range(1, 12)
                .Select(m => new RevenueByMonthViewModel
                {
                    Year = currentYear,
                    Month = m,
                    Revenue = revenueData.FirstOrDefault(r => r.Year == currentYear && r.Month == m)?.Revenue ?? 0
                })
                .ToList();

            // Truyền dữ liệu đến View
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.OrdersByStatus = ordersByStatus;
            ViewBag.RevenueByMonth = fullRevenueByMonth;

            return View();
        }


        public ActionResult ManageProduct()
        {
            var products = _productService.GetAllProducts();
            return View(products);
        }

        public ActionResult AddProduct()
        {
            ViewBag.Categories = new SelectList(_db.Categories, "CategoryID", "CategoryName");
            return View(new Product()); // Đảm bảo truyền một đối tượng Product rỗng
        }
        [HttpPost]
        public ActionResult AddProduct(Product product, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (_productService.AddProduct(product))
                {
                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                        string path = Path.Combine(Server.MapPath("~/Content/products/"), fileName);
                        imageFile.SaveAs(path);
                        product.ImageURL = "/Content/products/" + fileName;
                        // Cập nhật lại sản phẩm sau khi có đường dẫn ảnh
                        _productService.UpdateProduct(product);
                    }
                    TempData["SuccessMessage"] = "Thêm sản phẩm thành công!";
                    return RedirectToAction("ManageProduct");
                }
                ViewBag.Error = "Tên sản phẩm đã tồn tại.";
            }
            return View(product);
        }

        public ActionResult EditProduct(int id)
        {
            var product = _productService.GetProductById(id);
            if (product == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy sản phẩm.";
                return RedirectToAction("ManageProduct");
            }
            ViewBag.Categories = new SelectList(_db.Categories, "CategoryID", "CategoryName", product.CategoryID);

            return View(product);
        }

        [HttpPost]
        public ActionResult EditProduct(Product model, HttpPostedFileBase ImageUpload)
        {
            if (ModelState.IsValid)
            {
                if (ImageUpload != null && ImageUpload.ContentLength > 0)
                {
                    string fileName = Path.GetFileName(ImageUpload.FileName);
                    string path = Path.Combine(Server.MapPath("~/Content/products/"), fileName);
                    ImageUpload.SaveAs(path);
                    model.ImageURL = "/Content/products/" + fileName;
                }

                var repo = new ProductRepository();
                if (repo.UpdateProduct(model))
                {
                    TempData["SuccessMessage"] = "Sản phẩm đã được cập nhật!";
                    return RedirectToAction("ManageProduct");
                }
            }

            ViewBag.Categories = new SelectList(DatabaseContextSingleton.Instance.Categories, "CategoryID", "CategoryName", model.CategoryID);
            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteProduct(int id)
        {
            if (_productService.DeleteProduct(id))
            {
                TempData["SuccessMessage"] = "Sản phẩm đã được xóa.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy sản phẩm.";
            }
            return RedirectToAction("ManageProduct");
        }


        public ActionResult ManageCategory()
        {
            var categories = DatabaseContextSingleton.Instance.Categories.ToList();
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
                var db = DatabaseContextSingleton.Instance;

                var check = db.Categories.FirstOrDefault(c => c.CategoryName == category.CategoryName);
                if (check == null)
                {
                    var newCategory = new Category { CategoryName = category.CategoryName };
                    db.Categories.Add(newCategory);
                    db.SaveChanges();

                    return RedirectToAction("ManageCategory");
                }
                else
                {
                    ViewBag.Error = "Thể loại đã tồn tại.";
                }
            }
            return View(category);
        }

        public ActionResult EditCategory(int id)
        {
            var category = DatabaseContextSingleton.Instance.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        [HttpPost]
        public ActionResult EditCategory(Category model)
        {
            if (ModelState.IsValid)
            {
                var db = DatabaseContextSingleton.Instance;
                var category = db.Categories.Find(model.CategoryID);
                if (category != null)
                {
                    category.CategoryName = model.CategoryName;
                    db.SaveChanges();
                    return RedirectToAction("ManageCategory");
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult DeleteCategory(int id)
        {
            var db = DatabaseContextSingleton.Instance;
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

        private readonly CustomerRepository _customerRepo = new CustomerRepository();
        private readonly AdminRepository _adminRepo = new AdminRepository();
        public ActionResult ManagerUser()
        {
            var customers = _customerRepo.GetAllUsers();
            var admins = _adminRepo.GetAllUsers();
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
                string avatarPath = "/Content/images/7.jpg";

                if (avatarFile != null && avatarFile.ContentLength > 0)
                {
                    string uploadDir = Server.MapPath("~/Content/avatars/");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatarFile.FileName);
                    string filePath = Path.Combine(uploadDir, fileName);
                    avatarFile.SaveAs(filePath);

                    avatarPath = "/Content/avatars/" + fileName;
                }

                customer.Avatar = avatarPath;
                customer.CreatedAt = DateTime.Now;

                if (_customerRepo.Add(customer))
                {
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
            var customer = _customerRepo.GetById(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        [HttpPost]
        public ActionResult EditCustomer(int id, Customer customer, HttpPostedFileBase avatarFile)
        {
            if (ModelState.IsValid)
            {
                if (avatarFile != null && avatarFile.ContentLength > 0)
                {
                    string uploadDir = Server.MapPath("~/Content/avatars/");
                    if (!Directory.Exists(uploadDir))
                    {
                        Directory.CreateDirectory(uploadDir);
                    }

                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatarFile.FileName);
                    string filePath = Path.Combine(uploadDir, fileName);
                    avatarFile.SaveAs(filePath);

                    customer.Avatar = "/Content/avatars/" + fileName;
                }

                if (_customerRepo.Update(id, customer))
                {
                    return RedirectToAction("ManagerUser");
                }
                else
                {
                    ViewBag.Error = "Email đã được sử dụng bởi khách hàng khác.";
                }
            }

            return View(customer);
        }

        [HttpPost]
        public ActionResult DeleteCustomer(int id)
        {
            if (_customerRepo.Delete(id))
            {
                TempData["SuccessMessage"] = "Người dùng đã được xóa thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Không thể xóa! Cần ít nhất một Admin.";
            }
            return RedirectToAction("ManagerUser");
        }

        public ActionResult ManagerOrder()
        {
            var orders = _db.Orders
                           .OrderByDescending(o => o.OrderDate)
                           .Include("OrderDetails.Product") // Load chi tiết đơn hàng
                           .ToList();
            return View(orders);
        }

        public ActionResult OrderDetails(int id)
        {
            var order = _db.Orders
                          .Include("OrderDetails.Product") // Load luôn thông tin sản phẩm
                          .FirstOrDefault(o => o.OrderID == id);

            if (order == null)
            {
                return HttpNotFound(); // Trả về lỗi 404 nếu không tìm thấy
            }

            return PartialView("OrderDetails", order); // Trả về PartialView chứa thông tin đơn hàng
        }


        [HttpPost]
        public JsonResult ConfirmOrder(int id)
        {
            try
            {
                var order = _db.Orders.Find(id);
                if (order != null)
                {
                    // Kiểm tra nếu đơn hàng đã bị hủy hoặc đã giao thì không cho thay đổi
                    if (order.Status == "Đã hủy" || order.Status == "Đã giao")
                    {
                        return Json(new { success = false, message = "Không thể xác nhận đơn hàng đã bị hủy hoặc đã giao." });
                    }

                    order.Status = "Đã giao";
                    _db.SaveChanges();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Không tìm thấy đơn hàng." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.InnerException?.Message ?? ex.Message });
            }
        }


        [HttpPost]
        public JsonResult CancelOrder(int id)
        {
            try
            {
                var order = _db.Orders.Find(id);
                if (order != null)
                {
                    // Kiểm tra nếu đơn hàng đã bị hủy hoặc đã giao thì không cho thay đổi
                    if (order.Status == "Đã hủy" || order.Status == "Đã giao")
                    {
                        return Json(new { success = false, message = "Không thể hủy đơn hàng đã bị hủy hoặc đã giao." });
                    }

                    order.Status = "Đã hủy";

                    // Lấy danh sách sản phẩm trong đơn hàng
                    var orderDetails = _db.OrderDetails.Where(od => od.OrderID == id).ToList();

                    foreach (var item in orderDetails)
                    {
                        var product = _db.Products.Find(item.ProductID);
                        if (product != null)
                        {
                            product.Stock += item.Quantity; // Hoàn trả số lượng sản phẩm
                        }
                    }

                    _db.SaveChanges();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Không tìm thấy đơn hàng." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.InnerException?.Message ?? ex.Message });
            }
        }
    }
}
