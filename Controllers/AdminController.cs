using System.Linq;
using System.Web.Mvc;
using WebMusic.Models;
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
        public ActionResult AddCustomer()
        {
            return View();
        }
        [HttpPost]
        public ActionResult AddCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra email đã tồn tại chưa
                var existingCustomer = _db.Customers.FirstOrDefault(c => c.Email == customer.Email);
                if (existingCustomer != null)
                {
                    ViewBag.Error = "Email này đã được sử dụng.";
                    return View(customer);
                }

                _db.Customers.Add(customer);
                _db.SaveChanges();
                TempData["SuccessMessage"] = "Thêm khách hàng thành công!";
                return RedirectToAction("ManagerUser");
            }
            return View(customer);
        }

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Dashboard()
        {
            return View();
        }
        public ActionResult ManagerUser()
        {
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
        public ActionResult AddProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                if (_productService.AddProduct(product))
                {
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
            return View(product);
        }

        [HttpPost]
        public ActionResult EditProduct(Product product)
        {
            if (ModelState.IsValid)
            {
                if (_productService.UpdateProduct(product))
                {
                    TempData["SuccessMessage"] = "Cập nhật sản phẩm thành công!";
                    return RedirectToAction("ManageProduct");
                }
                ViewBag.Error = "Cập nhật sản phẩm thất bại.";
            }
            return View(product);
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
    }
}
