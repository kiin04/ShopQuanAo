using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMusic.Models;

namespace WebMusic.Repositories
{
    public interface ICategoryRepository
    {
        IQueryable<Category> GetAll();
    }
}