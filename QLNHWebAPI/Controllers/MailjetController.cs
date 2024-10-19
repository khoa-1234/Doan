using Mailjet.Client;
using Mailjet.Client.Resources;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using QLNHWebAPI.Service;
using static System.Net.WebRequestMethods;

namespace QLNHWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailjetController:ControllerBase
    {
        private readonly MailjetService _mailjetService;

        public MailjetController(MailjetService mailjetService)
        {
            _mailjetService = mailjetService;
        }

        [HttpPost("send-confirmation-email")]
        public async Task<IActionResult> SendConfirmationEmail(string toEmail, string token)
        {
            var subject = "Verify Your Email Address";
            var body = $"<!DOCTYPE html>\r\n<html lang=\"vi\">\r\n<head>\r\n  " +
                $"  <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" " +
                $"content=\"width=device-width, initial-scale=1.0\">\r\n    <style>\r\n      " +
                $"  body {{\r\n            font-family: Arial, sans-serif;\r\n           " +
                $" background-color: #f4f4f4;\r\n            margin: 0;\r\n            padding: 0;\r\n   " +
                $"     }}\r\n        .container {{\r\n            max-width: 600px;\r\n            margin: 0 auto;\r\n          " +
                $"  padding: 20px;\r\n            background-color: #ffffff;\r\n            border-radius: 5px;\r\n          " +

                $"  box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);\r\n        }}\r\n        .header {{\r\n            background-color: #28a745;\r\n       " +
                $"     color: #ffffff;\r\n            padding: 10px 20px;\r\n            border-radius: 5px 5px 0 0;\r\n            text-align: center;\r\n    " +
                $"    }}\r\n        .content {{\r\n            padding: 20px;\r\n        }}\r\n        .otp {{\r\n            font-size: 24px;\r\n         " +
                $"   font-weight: bold;\r\n            color: #333333;\r\n            text-align: center;\r\n            margin: 20px 0;\r\n        }}\r\n        .footer {{\r\n        " +
                $"    font-size: 12px;\r\n            color: #888888;\r\n            text-align: center;\r\n            margin-top: 20px;\r\n        }}\r\n        .footer a {{\r\n           " +
                $" color: #28a745;\r\n            text-decoration: none;\r\n        }}\r\n    </style>\r\n</head>\r\n<body>\r\n    <div class=\"container\">\r\n        <div class=\"header\">\r\n   " +
                $"         <h1>Xác Nhận Đặt Bàn</h1>\r\n        </div>\r\n        <div class=\"content\">\r\n         " +
                $"   <p>Chào bạn,</p>\r\n           " +
                $" <p>Cảm ơn {toEmail} đã đặt bàn với chúng tôi. Để hoàn tất việc đặt bàn, vui lòng sử dụng mã OTP sau:</p>\r\n          " +
                $"  <div class=\"otp\">{token}</div>\r\n          " +
                $"  <p>Nhập mã này trên trang web để hoàn tất việc đặt bàn. Nếu bạn không thực hiện đặt bàn này, vui lòng bỏ qua email này.</p>\r\n        " +
                $"</div>\r\n        <div class=\"footer\">\r\n            <p>Trân trọng,</p>\r\n            <p>Tên Nhà Hàng Của Bạn</p>\r\n         " +
                $"   <p>Truy cập <a href=\"https://yourwebsite.com\">website</a> của chúng tôi để biết thêm thông tin.</p>\r\n        </div>\r\n    </div>\r\n</body>\r\n</html>";

            var result = await _mailjetService.SendEmailAsync(toEmail, subject, body);
            if (result)
            {
                return Ok("Confirmation email sent.");
            }
            return StatusCode(500, "Failed to send confirmation email.");
        }
    }
}
