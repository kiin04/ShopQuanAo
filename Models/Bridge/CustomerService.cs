using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMusic.Models.Bridge
{
    public class CustomerService : ICustomerService
    {
        //CheckoutController ThanhToan
        private readonly ShopQuanAoEntities _db;

        public CustomerService(ShopQuanAoEntities db)
        {
            _db = db;
        }

        public Customer GetCustomerFromSession(HttpSessionStateBase session)
        {
            var cus = session["Customer"] as Customer;
            if (cus == null)
            {
                int? customerId = session["CustomerID"] as int?;
                if (customerId != null)
                {
                    cus = _db.Customers.Find(customerId);
                    session["Customer"] = cus;
                }
            }
            return cus;
        }
    }
}