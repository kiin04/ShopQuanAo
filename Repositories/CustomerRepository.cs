using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using WebMusic.Models;

namespace WebMusic.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly ShopQuanAoEntities _context;

        public CustomerRepository(ShopQuanAoEntities context)
        {
            _context = context;
        }

        public Customer GetCustomerById(int customerId)
        {
            return _context.Customers.FirstOrDefault(c => c.CustomerID == customerId);
        }

        public void UpdateCustomer(Customer customer)
        {
            _context.Entry(customer).State = EntityState.Modified;
            _context.SaveChanges();
        }
    }


}