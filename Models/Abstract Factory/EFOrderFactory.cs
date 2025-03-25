using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMusic.Models.Abstract_Factory
{
    public class EFOrderFactory : IOrderFactory
    {
        //ProfileController OrderHistory OrderDetail
        private readonly ShopQuanAoEntities _db;

        public EFOrderFactory(ShopQuanAoEntities db)
        {
            _db = db;
        }

        public List<Order> GetOrders(int userId)
        {
            return _db.Orders
                      .Where(o => o.CustomerID == userId) 
                      .OrderByDescending(o => o.OrderDate)
                      .ToList();
        }

        public Order GetOrderDetails(int orderId)
        {
            return _db.Orders.Include("OrderDetails.Product")
                             .FirstOrDefault(o => o.OrderID == orderId);
        }
    }
}