using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMusic.Models;
using PagedList;
using WebMusic.Repositories;

namespace WebMusic.Controllers
{
    public class DanhMucController : Controller
    {

        private readonly IUnitOfWork _unitOfWork;

        public DanhMucController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        ShopQuanAoEntities db = new ShopQuanAoEntities();
        // GET: DanhMuc
        public ActionResult DanhMuc(int? categoryId, string sortOrder, int? page)
        {
            int pageSize = 15; // Số sản phẩm mỗi trang
            int pageNumber = (page ?? 1); // Trang mặc định
            //unit
            var products = _unitOfWork.ProductRepository.GetAll().AsQueryable();



            // Lọc theo danh mục
            if (categoryId.HasValue && categoryId != 0)
            {
                products = products.Where(p => p.CategoryID == categoryId);
            }
            ViewBag.SelectedCategoryId = categoryId;

            // Sắp xếp theo giá hoặc mặc định theo ProductID
            switch (sortOrder)
            {
                case "price_asc":
                    products = products.OrderBy(p => p.Price);
                    ViewBag.SelectedSort = "price_asc";
                    break;
                case "price_desc":
                    products = products.OrderByDescending(p => p.Price);
                    ViewBag.SelectedSort = "price_desc";
                    break;
                default:
                    products = products.OrderBy(p => p.ProductID); // Sắp xếp mặc định để dùng Skip()
                    ViewBag.SelectedSort = null;
                    break;
            }

            // Sử dụng CategoryRepository từ UnitOfWork để lấy danh sách categories
            ViewBag.Categories = _unitOfWork.CategoryRepository.GetAll().ToList();

            // Áp dụng phân trang
            return View(products.ToPagedList(pageNumber, pageSize));
        }

    }
}