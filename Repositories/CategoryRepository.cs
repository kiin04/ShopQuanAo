using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMusic.Models;

namespace WebMusic.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ShopQuanAoEntities _context;

        public CategoryRepository(ShopQuanAoEntities context)
        {
            _context = context;
        }

        public IQueryable<Category> GetAll()
        {
            return _context.Categories.AsQueryable();
        }
    }

}