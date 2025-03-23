using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMusic.Models
{
    public class CustomerRepository : BaseUserRepository<Customer>
    {
        public override List<UserViewModel> GetAllUsers()
        {
            //AdminController ManagerUser (Add,Update,Delete)

            return _db.Customers.Select(c => new UserViewModel
            {
                ID = c.CustomerID,
                FullName = c.FullName,
                Email = c.Email,
                Phone = c.Phone,
                Address = c.Address,
                Role = "Customer",
                Avatar = c.Avatar
            }).ToList();
        }

        public override Customer GetById(int id)
        {
            return _db.Customers.Find(id);
        }

        public override bool Add(Customer customer)
        {
            if (_db.Customers.Any(c => c.Email.Trim() == customer.Email.Trim()))
            {
                return false;
            }

            _db.Customers.Add(customer);
            _db.SaveChanges();
            return true;
        }

        public override bool Update(int id, Customer customer)
        {
            var existingCustomer = _db.Customers.Find(id);
            if (existingCustomer == null) return false;

            if (_db.Customers.Any(c => c.Email.Trim() == customer.Email.Trim() && c.CustomerID != id))
            {
                return false;
            }

            existingCustomer.FullName = customer.FullName.Trim();
            existingCustomer.Email = customer.Email.Trim();
            existingCustomer.Phone = customer.Phone.Trim();
            existingCustomer.Address = customer.Address.Trim();
            existingCustomer.Avatar = customer.Avatar;

            _db.SaveChanges();
            return true;
        }

        public override bool Delete(int id)
        {
            var customer = _db.Customers.Find(id);
            if (customer == null) return false;

            _db.Customers.Remove(customer);
            _db.SaveChanges();
            return true;
        }
    }

}