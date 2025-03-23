using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMusic.Models;
using WebMusic.Models.State;

namespace WebMusic.Controllers
{
    public class SearchController : Controller
    {
        ShopQuanAoEntities db = new ShopQuanAoEntities();
        // GET: Search
        public ActionResult Search(string keyword)
        {
            var searchState = new SearchState(db);
            searchState.SearchProducts(keyword);

            ViewBag.Keyword = keyword;
            return View(searchState.Products);
        }

        public JsonResult GetSuggestions(string keyword)
        {
            var searchState = new SearchState(db);
            searchState.GetSuggestions(keyword);
            return Json(searchState.Suggestions, JsonRequestBehavior.AllowGet);
        }
    }
}