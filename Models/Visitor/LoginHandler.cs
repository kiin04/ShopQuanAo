using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMusic.Models.Visitor
{
    public class LoginHandler : ILoginVisitor
    {
        //LoginController

        private HttpSessionStateBase _session;

        public LoginHandler(HttpSessionStateBase session)
        {
            _session = session;
        }

        public void Visit(Customer customer)
        {
            _session["Customer"] = customer;
            _session["Avatar"] = customer.Avatar ?? "/Content/images/profile1.png";
            _session["Email"] = customer.Email;
            _session["CurrentUserId"] = customer.CustomerID;
            _session["FullName"] = customer.FullName;
            _session["Role"] = "Customer";
            _session["Phone"] = customer.Phone;
            _session["Address"] = customer.Address;
        }

        public void Visit(User admin)
        {
            _session["Email"] = admin.Email;
            _session["CurrentUserId"] = admin.UserID;
            _session["Username"] = admin.Username;
            _session["Role"] = "Admin";
        }
    }
}