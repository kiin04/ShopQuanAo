using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMusic.Models
{
    public abstract class BaseUserRepository<T> where T : class
    {
        //AdminController ManagerUser (Add,Update,Delete)

        protected readonly ShopQuanAoEntities _db;

        protected BaseUserRepository()
        {
            _db = new ShopQuanAoEntities();
        }

        public abstract List<UserViewModel> GetAllUsers();
        public abstract T GetById(int id);
        public abstract bool Add(T entity);
        public abstract bool Update(int id, T entity);
        public abstract bool Delete(int id);
    }
}