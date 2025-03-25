using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMusic.Models.BuilderPattern
{
    public class OrderBuilder
    {
        //AdminController ManagerOrder
        private readonly ShopQuanAoEntities _db;
        private Order _order;

        public OrderBuilder(ShopQuanAoEntities db, int orderId)
        {
            _db = db;
            _order = _db.Orders.Include("OrderDetails.Product").FirstOrDefault(o => o.OrderID == orderId);
        }

        public bool OrderExists() => _order != null;
        public string GetOrderStatus() => _order?.Status;
        public Order Build() => _order;

        public OrderBuilder ConfirmOrder()
        {
            if (_order != null && _order.Status != "Đã hủy" && _order.Status != "Đã giao")
            {
                _order.Status = "Đã giao";
            }
            return this;
        }

        public OrderBuilder CancelOrder()
        {
            if (_order != null && _order.Status != "Đã hủy" && _order.Status != "Đã giao")
            {
                _order.Status = "Đã hủy";
                foreach (var item in _order.OrderDetails)
                {
                    var product = _db.Products.Find(item.ProductID);
                    if (product != null)
                    {
                        product.Stock += item.Quantity; // Hoàn trả số lượng sản phẩm
                    }
                }
            }
            return this;
        }

        public void SaveChanges()
        {
            if (_order != null)
            {
                _db.SaveChanges();
            }
        }
    }
}