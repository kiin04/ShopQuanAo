using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMusic.Models;

namespace WebMusic.Repositories
{
    public interface IUserRepository
    {
        User GetUserById(int userId);
        void UpdateUser(User user);
    }

}