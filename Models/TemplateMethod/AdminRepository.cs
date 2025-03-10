using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMusic.Models.TemplateMethod
{
    public class AdminRepository : BaseUserRepository<User>
    {
        public override List<UserViewModel> GetAllUsers()
        {
            return _db.Users.Select(u => new UserViewModel
            {
                ID = u.UserID,
                FullName = u.Username,
                Email = u.Email,
                Phone = "-",
                Address = "-",
                Role = "Admin",
                Avatar = "/Content/images/7.jpg"
            }).ToList();
        }

        public override User GetById(int id)
        {
            return _db.Users.Find(id);
        }

        public override bool Add(User admin)
        {
            if (_db.Users.Any(u => u.Email.Trim() == admin.Email.Trim()))
            {
                return false;
            }

            _db.Users.Add(admin);
            _db.SaveChanges();
            return true;
        }

        public override bool Update(int id, User admin)
        {
            var existingAdmin = _db.Users.Find(id);
            if (existingAdmin == null) return false;

            if (_db.Users.Any(u => u.Email.Trim() == admin.Email.Trim() && u.UserID != id))
            {
                return false;
            }

            existingAdmin.Username = admin.Username;
            existingAdmin.Email = admin.Email;

            _db.SaveChanges();
            return true;
        }

        public override bool Delete(int id)
        {
            var admin = _db.Users.Find(id);
            if (admin == null) return false;

            int adminCount = _db.Users.Count(u => u.Role == "Admin");
            if (adminCount <= 1) return false;

            _db.Users.Remove(admin);
            _db.SaveChanges();
            return true;
        }
    }

}