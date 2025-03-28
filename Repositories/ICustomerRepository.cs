using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMusic.Models;

namespace WebMusic.Repositories
{
    public interface ICustomerRepository
    {
        Customer GetCustomerById(int customerId);
        void UpdateCustomer(Customer customer);
    }

}