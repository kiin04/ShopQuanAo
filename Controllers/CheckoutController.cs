using PayPal.Api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebMusic.Models;
using WebMusic.Models.Bridge;

namespace WebMusic.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ShopQuanAoEntities db;
        private readonly ICustomerService _customerService;

        public CheckoutController()
        {
            db = new ShopQuanAoEntities();
            _customerService = new CustomerService(db);
        }

        public CheckoutController(ICustomerService customerService)
        {
            db = new ShopQuanAoEntities();
            _customerService = customerService;
        }
        // GET: Checkout
        public ActionResult ThanhToan()
        {
            var cart = Session["Cart"] as List<Cart>;
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("DanhMuc", "DanhMuc");
            }

            var cus = _customerService.GetCustomerFromSession(Session);

            if (cus != null)
            {
                ViewBag.FullName = cus.FullName;
                ViewBag.Email = cus.Email;
                ViewBag.Address = cus.Address;
                ViewBag.PhoneNumber = cus.Phone;
            }

            ViewBag.CartItems = cart;
            ViewBag.TotalAmount = cart.Sum(item => item.Quantity * item.Product.Price);
            return View();
        }
        // Xác nhận đặt hàng
        [HttpPost]
        public ActionResult ConfirmOrder(string FullName, string Address, string PhoneNumber, string PaymentMethod)
        {
            var cart = Session["Cart"] as List<Cart>;
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Cart", "Cart");
            }

            var customer = Session["Customer"] as Customer;
            int? customerId = customer != null ? customer.CustomerID : (int?)null;

            // Tạo đơn hàng mới
            Models.Order newOrder = new Models.Order
            {
                CustomerID = customerId, // Nếu có đăng nhập thì lấy ID khách hàng, nếu không thì null
                OrderDate = DateTime.Now,
                TotalAmount = cart.Sum(item => item.Quantity * item.Product.Price),
                Status = "Chờ xác nhận"
            };
            // Lưu newOrder vào Session để sử dụng sau khi thanh toán
            Session["PendingOrder"] = newOrder;
            db.Orders.Add(newOrder);
            db.SaveChanges();

            // Lưu chi tiết đơn hàng và cập nhật Stock sản phẩm
            foreach (var item in cart)
            {
                // Tạo chi tiết đơn hàng
                OrderDetail orderDetail = new OrderDetail
                {
                    OrderID = newOrder.OrderID,
                    ProductID = item.Product.ProductID,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                };
                db.OrderDetails.Add(orderDetail);

                // Giảm số lượng tồn kho của sản phẩm
                var product = db.Products.Find(item.Product.ProductID);
                if (product != null)
                {
                    // Đảm bảo số lượng tồn kho không bị âm
                    if (product.Stock >= item.Quantity)
                    {
                        product.Stock -= item.Quantity;
                    }
                    else
                    {
                        return Content($"Sản phẩm {product.ProductName} không đủ số lượng trong kho!");
                    }
                }
            }

            db.SaveChanges();

            // Nếu khách hàng đã đăng nhập và có giỏ hàng trong DB, xóa giỏ hàng của họ
            if (customerId.HasValue)
            {
                // Tìm và xóa các mặt hàng trong giỏ hàng của khách hàng từ cơ sở dữ liệu
                var cartItemsInDb = db.Carts.Where(c => c.CustomerID == customerId);
                db.Carts.RemoveRange(cartItemsInDb); // Xóa các sản phẩm trong giỏ hàng của khách hàng
                db.SaveChanges();
            }

            // Xóa giỏ hàng trong session
            Session["Cart"] = null;
            Session["CartCount"] = 0;
            return RedirectToAction("OrderSuccess", new { id = newOrder.OrderID });
        }


        // Hiển thị trang đặt hàng thành công
        public ActionResult OrderSuccess(int id)
        {
            var order = db.Orders
                .Include("OrderDetails.Product") // Load chi tiết sản phẩm
                .Include("Customer") // Load thông tin khách hàng
                .FirstOrDefault(o => o.OrderID == id);

            if (order == null)
            {
                return HttpNotFound();
            }

            return View(order);
        }

        // GET: Payment
        public ActionResult PaymentWithPaypal(string Cancel = null)
        {
            //getting the apiContext  
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class  
                    // Creating a payment  
                    // baseURL is the url on which paypal sendsback the data.  
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Checkout/PaymentWithPayPal?";
                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid  
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment  
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    //If executed payment failed then we will show payment failure message to user  
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                }
            }
            catch (Exception ex)
            {
                return View("FailureView");
            }
            //on successful payment, show success page to user.  

            var cart = Session["Cart"] as List<Cart>;
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Cart", "Cart");
            }

            var customer = Session["Customer"] as Customer;
            int? customerId = customer != null ? customer.CustomerID : (int?)null;

            // Tạo đơn hàng mới
            Models.Order newOrder = new Models.Order
            {
                CustomerID = customerId, // Nếu có đăng nhập thì lấy ID khách hàng, nếu không thì null
                OrderDate = DateTime.Now,
                TotalAmount = cart.Sum(item => item.Quantity * item.Product.Price),
                Status = "Chờ xác nhận"
            };
            // Lưu newOrder vào Session để sử dụng sau khi thanh toán
            Session["PendingOrder"] = newOrder;
            db.Orders.Add(newOrder);
            db.SaveChanges();

            // Lưu chi tiết đơn hàng và cập nhật Stock sản phẩm
            foreach (var item in cart)
            {
                // Tạo chi tiết đơn hàng
                OrderDetail orderDetail = new OrderDetail
                {
                    OrderID = newOrder.OrderID,
                    ProductID = item.Product.ProductID,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                };
                db.OrderDetails.Add(orderDetail);

                // Giảm số lượng tồn kho của sản phẩm
                var product = db.Products.Find(item.Product.ProductID);
                if (product != null)
                {
                    // Đảm bảo số lượng tồn kho không bị âm
                    if (product.Stock >= item.Quantity)
                    {
                        product.Stock -= item.Quantity;
                    }
                    else
                    {
                        return Content($"Sản phẩm {product.ProductName} không đủ số lượng trong kho!");
                    }
                }
            }

            db.SaveChanges();

            // Nếu khách hàng đã đăng nhập và có giỏ hàng trong DB, xóa giỏ hàng của họ
            if (customerId.HasValue)
            {
                // Tìm và xóa các mặt hàng trong giỏ hàng của khách hàng từ cơ sở dữ liệu
                var cartItemsInDb = db.Carts.Where(c => c.CustomerID == customerId);
                db.Carts.RemoveRange(cartItemsInDb); // Xóa các sản phẩm trong giỏ hàng của khách hàng
                db.SaveChanges();
            }

            // Xóa giỏ hàng trong session
            Session["Cart"] = null;
            Session["CartCount"] = 0;
            return RedirectToAction("OrderSuccess", new { id = newOrder.OrderID });

        }
        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {
            var listSP = Session["Cart"] as List<Cart>;

            //create itemlist and add item objects to it  
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };

            foreach (var item in listSP)
            {
                itemList.items.Add(new Item()
                {
                   name = item.Product.ProductName,
                   currency = "USD",
                   price = Math.Round(item.Product.Price / 25600 , 2).ToString(),
                   quantity = item.Quantity.ToString(),
                   sku = item.Product.ProductID.ToString()
                });
            }

            //Adding Item Details like name, currency, price etc  
            
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = listSP.Sum(item => item.Quantity * (Math.Round(item.Product.Price / 25600, 2))).ToString()
            };
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = listSP.Sum(item => item.Quantity * (Math.Round(item.Product.Price / 25600, 2))).ToString(), 
                details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            var paypalOrderId = DateTime.Now.Ticks;
            transactionList.Add(new Transaction()
            {
                description = $"Invoice #{paypalOrderId}",
                invoice_number = paypalOrderId.ToString(), //Generate an Invoice No    
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }

        public ActionResult FailureView()
        {
            return View();
        }
    }
}

