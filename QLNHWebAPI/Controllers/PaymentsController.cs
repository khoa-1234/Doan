using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QLNHWebAPI.Models;
using QLNHWebAPI.VNPay;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QLNHWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentsController : ControllerBase
    {
        private readonly QlnhContext _context;
        private readonly VnPayService _vnPayService;

        public PaymentsController(QlnhContext context, VnPayService vnPayService)
        {
            _context = context;
            _vnPayService = vnPayService;
        }

        [HttpPost("create-payment")]
        public async Task<IActionResult> CreatePayment([FromBody] PaymentRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.OrderId) || request.Amount <= 0)
            {
                return BadRequest(new { message = "Thông tin thanh toán không hợp lệ." });
            }

            try
            {
                var paymentUrl = await _vnPayService.CreatePaymentRequest(request.OrderId, request.Amount, request.OrderInfo);
                return Ok(new { paymentUrl });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = $"Lỗi: {ex.Message}" });
            }
        }

        [HttpGet("vnpay_return")]
        public IActionResult VnPayReturn([FromQuery] SortedDictionary<string, string> queryParameters)
        {
            bool isValidSignature = _vnPayService.ValidateSignature(queryParameters);
            if (isValidSignature)
            {
                // Cập nhật trạng thái đơn hàng tại đây
                // var order = await _context.Orders.FindAsync(queryParameters["vnp_OrderId"]);
                // if (order != null) { order.Status = "Thành công"; await _context.SaveChangesAsync(); }

                return Ok(new { message = "Giao dịch thành công", parameters = queryParameters });
            }
            else
            {
                return BadRequest(new { message = "Giao dịch không hợp lệ (Invalid Signature)" });
            }
        }

        [HttpPost("vnpay_notify")]
        public IActionResult VnPayNotify([FromQuery] SortedDictionary<string, string> queryParameters)
        {
            bool isValidSignature = _vnPayService.ValidateSignature(queryParameters);
            if (isValidSignature)
            {
                // Xử lý thông báo thanh toán thành công
                // var order = await _context.Orders.FindAsync(queryParameters["vnp_OrderId"]);
                // if (order != null) { order.Status = "Đã thanh toán"; await _context.SaveChangesAsync(); }

                return Ok(new { message = "Giao dịch thành công", parameters = queryParameters });
            }
            else
            {
                return BadRequest(new { message = "Giao dịch không hợp lệ (Invalid Signature)" });
            }
        }
    }

    public class PaymentRequest
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string OrderInfo { get; set; }
    }
}
