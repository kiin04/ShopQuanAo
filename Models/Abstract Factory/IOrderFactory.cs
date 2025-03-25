using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebMusic.Models.Abstract_Factory
{
    public interface IOrderFactory
    {
        //ProfileController OrderHistory OrderDetail

        List<Order> GetOrders(int userId);
        Order GetOrderDetails(int orderId);
    }
}
