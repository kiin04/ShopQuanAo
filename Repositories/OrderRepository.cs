using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebMusic.Models;

namespace WebMusic.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ShopQuanAoEntities _db;

        public OrderRepository(ShopQuanAoEntities db)
        {
            _db = db;
        }

        public List<Order> GetOrdersByCustomerId(int customerId)
        {
            return _db.Orders
                      .Where(o => o.CustomerID == customerId)
                      .OrderByDescending(o => o.OrderDate)
                      .ToList();
        }

        public Order GetOrderDetails(int orderId)
        {
            return _db.Orders
                      .Include("OrderDetails.Product")
                      .FirstOrDefault(o => o.OrderID == orderId);
        }
    }

}