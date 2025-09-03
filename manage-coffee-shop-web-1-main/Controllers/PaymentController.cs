/*using manage_coffee_shop_web.Models;  // Đảm bảo file Order.cs tồn tại
using manage_coffee_shop_web.Services;  // Đảm bảo file VnPayLibrary.cs tồn tại
using Microsoft.AspNetCore.Http;  // For HttpContext
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace manage_coffee_shop_web.Controllers {
    public class PaymentController : Controller {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public PaymentController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Action to initiate payment (call this after creating order from cart)
        [HttpPost]
        public IActionResult InitiatePayment(int orderId)  // Pass orderId from checkout
        {
            var order = _context.Orders.Find(orderId);
            if (order == null || order.Status != "Pending")
            {
                return BadRequest("Invalid order.");
            }

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", _configuration["VNPay:Version"]);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", _configuration["VNPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (order.OrderTotal * 100).ToString());  // VND * 100
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _configuration["VNPay:Currency"]);
            vnpay.AddRequestData("vnp_IpAddr", GetClientIpAddress());
            vnpay.AddRequestData("vnp_Locale", _configuration["VNPay:Locale"]);
            vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang {order.Id}");
            vnpay.AddRequestData("vnp_OrderType", "other");  // Adjust based on your goods category (e.g., "food")
            vnpay.AddRequestData("vnp_ReturnUrl", _configuration["VNPay:ReturnUrl"]);
            vnpay.AddRequestData("vnp_TxnRef", order.Id.ToString());  // Use order ID as ref

            var paymentUrl = vnpay.CreateRequestUrl(_configuration["VNPay:BaseUrl"], _configuration["VNPay:HashSecret"]);

            return Redirect(paymentUrl);
        }

        // Return URL handler
        [HttpGet]
        public IActionResult VnpayReturn()
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in Request.Query)
            {
                if (key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value);
                }
            }

            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_TransactionNo = vnpay.GetResponseData("vnp_TransactionNo");
            var vnp_SecureHash = Request.Query["vnp_SecureHash"];
            var vnp_OrderId = Convert.ToInt32(vnpay.GetResponseData("vnp_TxnRef"));
            var vnp_Amount = Convert.ToDecimal(vnpay.GetResponseData("vnp_Amount")) / 100;

            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _configuration["VNPay:HashSecret"]);

            if (checkSignature)
            {
                var order = _context.Orders.Find(vnp_OrderId);
                if (order != null && order.OrderTotal == vnp_Amount)
                {
                    if (vnp_ResponseCode == "00")
                    {
                        order.Status = "Paid";
                        order.TransactionId = vnp_TransactionNo;
                        _context.SaveChanges();
                        // Clear cart or notify user
                        return View("PaymentSuccess");
                    }
                    else
                    {
                        order.Status = "Failed";
                        _context.SaveChanges();
                        return View("PaymentFailure");
                    }
                }
            }

            return BadRequest("Invalid payment response.");
        }

        private string GetClientIpAddress()
        {
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "0.0.0.0";
        }
    }
}*/