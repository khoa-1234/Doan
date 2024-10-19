using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using QLNHWebAPI.Models;
using QLNHWebAPI.Service;
using QLNHWebAPI.ViewModel;
using QRCoder;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Net.Http;
using static QLNHWebAPI.Controllers.DatBanController;
using QRCode = QRCoder.QRCode;

namespace QLNHWebAPI.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class DatBanController : ControllerCustome

    {
        private readonly QlnhContext _context;
        private readonly MailjetService _mailjetService;

        // private readonly IResponseService _responseService;
        public DatBanController(QlnhContext context, MailjetService mailjetService) //IResponseService responseService)
        {
            _context = context;
            _mailjetService = mailjetService;

            //_responseService = responseService;
        }
        public class DatBanDTO
        {
            public int DatBanId { get; set; }
            public int KhachHangId { get; set; }
            public string HoTen { get; set; }  // Tên khách hàng
            public DateOnly? NgayDat { get; set; }
            public DateTime? ThoiGianDat { get; set; }
            public int? SoNguoi { get; set; }
            public string? PhuongThucDat { get; set; }
            public string? TrangThai { get; set; }
            public string? GhiChu { get; set; }
        }

        //Get ALL PUSER
        [HttpGet]
        public async Task<ActionResult<ResponeMessage>> GetAllDatBan()
        {
            var datBans = await _context.DatBans
       .Include(db => db.KhachHang) // Liên kết với bảng KhachHang
       .Select(db => new DatBanDTO
       {
           DatBanId = db.DatBanId,
           KhachHangId = db.KhachHangId ?? 0, // Kiểm tra null và gán giá trị mặc định
           HoTen = db.KhachHang.HoTen, // Lấy họ tên khách hàng
           NgayDat = db.NgayDat,
           ThoiGianDat = db.ThoiGianDat,
           SoNguoi = db.SoNguoi,
           PhuongThucDat = db.PhuongThucDat,
           TrangThai = db.TrangThai,
           GhiChu = db.GhiChu
       })
       .ToListAsync();

            return await ReturnMessagesucces(datBans);
        }
        public class KiemtrabantrongDTO
        {
             public  DateOnly? ngayDat { get; set; }
           public  DateTime? thoiGianDat { get; set; }
        }

        [HttpGet("{banId}/latest")]
        public async Task<IActionResult> GetLatestDatBan(int banId)
        {
            // Truy vấn để lấy tất cả DatBan dựa trên BanId
            var latestDatBanList = await _context.DatBans
                .Where(db => db.BanId == banId)
                .OrderByDescending(db => db.ThoiGianDat) // Sắp xếp theo ThoiGianDat
                .ThenByDescending(db => db.NgayDat) // Sắp xếp theo NgayDat
                .ToListAsync(); // Lấy tất cả kết quả vào danh sách

            // Kiểm tra nếu không có kết quả, trả về thông báo không tìm thấy
            var latestDatBan = latestDatBanList.FirstOrDefault();
            if (latestDatBan == null)
            {
                return NotFound(new
                {
                    IsSuccess = false,
                    Message = $"Không tìm thấy thông tin đặt bàn cho BanId: {banId}"
                });
            }

            // Chuyển đổi Ngày Đặt sang DateTime nếu cần thiết
            DateTime? thoiGianDat = latestDatBan.ThoiGianDat ?? DateTime.Now;
            DateTime? ngayDat = latestDatBan.NgayDat.HasValue
                ? latestDatBan.NgayDat.Value.ToDateTime(TimeOnly.MinValue)
                : (DateTime?)null; // Chuyển đổi sau khi lấy dữ liệu

            // Trả về thông tin đặt bàn mới nhất bao gồm NhanVienMoBanId và KhachHangId
            return Ok(new
            {
                IsSuccess = true,
                Message = "Lấy thông tin đặt bàn thành công",
                Data = new
                {
                    DatBanId = latestDatBan.DatBanId,
                    BanId = latestDatBan.BanId,
                    NgayDat = ngayDat,
                    ThoiGianDat = thoiGianDat, // Gán giá trị hiện tại nếu ThoiGianDat null
                    SoNguoi = latestDatBan.SoNguoi,
                    TrangThai = latestDatBan.TrangThai,
                    GhiChu = latestDatBan.GhiChu,
                    KhachHangId = latestDatBan.KhachHangId, // Thêm KhachHangId vào phản hồi
                    NhanVienMoBanId = latestDatBan.NhanVienMoBanId // Thêm NhanVienMoBanId vào phản hồi
                }
            });
        }


        public class Bantrong
        {
            public int? BanId { get; set; }
            public int? SoGhe { get; set; }
        }



        [HttpPost("kiem-tra-ban-trong")]
        public async Task<ActionResult<ResponeMessage>> KiemTraBanTrong(KiemtrabantrongDTO banTrongDTO)
        {

            var ngayDat = banTrongDTO.ngayDat;
            var thoiGianDat = banTrongDTO.thoiGianDat;
            // Kiểm tra xem thoiGianDat có giá trị không
            if (!thoiGianDat.HasValue)
            {
                return BadRequest(new ResponeMessage
                {
                    Message = "Thời gian đặt không hợp lệ.",
                    IsSuccess = false
                });
            }

            // Tính toán thời gian bắt đầu và kết thúc
            var thoiGianBatDau = thoiGianDat.Value.AddHours(-4); // Lấy giá trị DateTime từ thoiGianDat
            var thoiGianKetThuc = thoiGianDat.Value.AddHours(4); // Lấy giá trị DateTime từ thoiGianDat

            // Lấy danh sách bàn trống
            var banTrongs = await _context.Bans
                .Where(b => !_context.DatBans.Any(db =>
                    db.BanId == b.BanId &&
                    db.NgayDat == ngayDat &&
                    (
                        (db.ThoiGianDat >= thoiGianBatDau && db.ThoiGianDat <= thoiGianKetThuc) ||
                        (db.ThoiGianDongBan == null || db.ThoiGianDongBan > thoiGianDat)
                    )))
                .ToListAsync();

            if (banTrongs.Count == 0)
            {
                return BadRequest(new ResponeMessage
                {
                    Message = "Không còn bàn trống.",
                    IsSuccess = false
                });
            }

            return await ReturnMessagesucces(banTrongs);
        }


        //[HttpGet("{id}")]
        // public async Task<ActionResult<SanPhamModalView>> GetSanPham(int id)

        //[HttpPost("Filter")]
        //public async Task<ActionResult<ResponeMessage>> PostSanPhamFilter(string? searchTerm)
        //{
        //    var sanphams = await _context.SanPhams
        //     .Where(p => p.TenSanPham.Contains(searchTerm) || p.LoaiSanPham.Contains(searchTerm))
        //     .ToListAsync();

        //    //return await _responseService.CreateResponseAsync(sanphams);
        //    return await ReturnMessagesucces(sanphams);

        //}
        [HttpPost("send-otp")]
        public async Task<ActionResult> CreateBooking([FromBody] SendOtpRequest request)
        {
           
            // Tạo mã OTP ngẫu nhiên
            var otpCode = GenerateOtpCode();

            // Lưu OTP vào cơ sở dữ liệu
            var otpEntity = new Otp
            {
                Email = request.Email,
                Otpcode = otpCode,
                ExpiryDate = DateTime.Now.AddMinutes(10) // OTP hợp lệ trong 10 phút
            };

            _context.Otps.Add(otpEntity);
            await _context.SaveChangesAsync();

            // Gửi OTP qua email
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
                $" <p>Cảm ơn {request.Hoten} đã đặt bàn với chúng tôi. Để hoàn tất việc đặt bàn, vui lòng sử dụng mã OTP sau:</p>\r\n          " +
                $"  <div class=\"otp\">{otpEntity.Otpcode}</div>\r\n          " +
                $"  <p>Nhập mã này trên trang web để hoàn tất việc đặt bàn. Nếu bạn không thực hiện đặt bàn này, vui lòng bỏ qua email này.</p>\r\n        " +
                $"</div>\r\n        <div class=\"footer\">\r\n            <p>Trân trọng,</p>\r\n            <p>Tên Nhà Hàng Của Bạn</p>\r\n         " +
                $"   <p>Truy cập <a href=\"https://yourwebsite.com\">website</a> của chúng tôi để biết thêm thông tin.</p>\r\n        </div>\r\n    </div>\r\n</body>\r\n</html>";

            var result = await _mailjetService.SendEmailAsync(otpEntity.Email, subject, body);
            if (result)
            {
                return Ok("Confirmation email sent.");
            }
            return StatusCode(500, "Failed to send confirmation email.");
        }
        public class SendOtpRequest
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }
            public string Hoten { get; set; }
        }
        private string GenerateOtpCode()
        {
            var random = new Random();
            var otpCode = random.Next(100000, 999999).ToString(); // Mã OTP 6 chữ số
            return otpCode;
        }

        //[HttpPost("confirm")]
        //public async Task<IActionResult> ConfirmOtp([FromBody] OTDDatBan datBan)
        //{
        //    // Kiểm tra OTP
        //    var otpSession = await _context.Otps
        //        .Where(o => o.Email == datBan.Email && o.Otpcode == datBan.OTP)
        //        .SingleOrDefaultAsync();

        //    if (otpSession == null || otpSession.ExpiryDate < DateTime.Now)
        //    {
        //        return BadRequest(new { Message = "OTP không hợp lệ hoặc đã hết hạn." });
        //    }



        //    // Kiểm tra khách hàng có tồn tại
        //    var khachHang = await _context.KhachHangs
        //  .SingleOrDefaultAsync(kh => kh.Email == datBan.Email);

        //    if (khachHang == null)
        //    {
        //        // Nếu khách hàng không tồn tại, tạo mới tài khoản người dùng và khách hàng
        //        var user = await _context.Pusers.SingleOrDefaultAsync(u => u.Username == datBan.Email);
        //        if (user == null)
        //        {
        //            user = new Puser
        //            {
        //                Username = otpSession.Email,
        //                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"), // Mã hóa mật khẩu 123456
        //                Status = 1,
        //                CreatedDate = DateTime.Now,
        //                Role = 2
        //            };
        //            _context.Pusers.Add(user);
        //            await _context.SaveChangesAsync();
        //        }

        //        // Tạo record KhachHang và lưu vào cơ sở dữ liệu
        //        khachHang = new KhachHang
        //        {
        //            UserId = user.UserId,
        //            HoTen = datBan.HoTen,
        //            SoDienThoai = datBan.SoDienThoai,
        //            Email = datBan.Email
        //        };
        //        _context.KhachHangs.Add(khachHang);
        //        await _context.SaveChangesAsync(); // Lưu KhachHang và lấy KhachHangId
        //    }

        //    // Lấy KhachHangId
        //    var khachHangId = khachHang.KhachHangId;


        //    // Lưu thông tin đặt bàn


        //    try
        //    {
        //        var datBanModel = new DatBan
        //        {
        //            KhachHangId = khachHangId,
        //            KhuVucId = datBan.KhuvucId, // Chỉ lưu khu vực
        //            NgayDat = datBan.NgayDat,
        //            ThoiGianDat = datBan.ThoiGianDat,
        //            SoNguoi = datBan.SoNguoi,
        //            GhiChu = datBan.GhiChu,
        //            PhuongThucDat = datBan.PhuongThucDat,
        //            TrangThai = "Chờ duyệt" // Trạng thái chờ xác nhận
        //        };
        //        // Tạo đơn hàng
        //        var donHang = new DonHang
        //        {
        //            DatBanId = datBanModel.DatBanId,
        //            NhanVienId = null, // Hoặc ID của nhân viên nào đó nếu cần
        //            NgayDat = DateOnly.FromDateTime(DateTime.Now),
        //            TongTien = 0, // Tính tổng tiền sau
        //            TrangThai = "Đang chờ xử lý",
        //            ChiTietDonHangs = new List<ChiTietDonHang>()
        //        };

        //        // Kiểm tra xem có đặt món không
        //        // Nếu có đặt món, set CoDatMon và thêm chi tiết đơn hàng
        //        if (datBan.MonAnDat != null && datBan.MonAnDat.Any())
        //        {
        //            datBanModel.CoDatMon = true; // Set CoDatMon = true
        //            foreach (var monAn in datBan.MonAnDat)
        //            {
        //                var chiTietDonHang = new ChiTietDonHang
        //                {
        //                    MonAnId = monAn.MonAnId,
        //                    SoLuong = monAn.SoLuong,
        //                    Gia = monAn.Gia,
        //                    TrangThai = "Chờ chế biến" // Trạng thái của món ăn
        //                };

        //                // Thêm chi tiết đơn hàng vào danh sách
        //                donHang.ChiTietDonHangs.Add(chiTietDonHang);
        //            }
        //            // Tính tổng tiền cho đơn hàng
        //            donHang.TongTien = donHang.ChiTietDonHangs.Sum(x => x.Gia * x.SoLuong);

        //        }
        //        else
        //        {
        //            datBanModel.CoDatMon = false; // Set CoDatMon = false nếu không có món
        //        }
        //        // Thêm đơn hàng vào cơ sở dữ liệu
        //        _context.DonHangs.Add(donHang);

        //        // Thêm đặt bàn vào cơ sở dữ liệu
        //        _context.DatBans.Add(datBanModel);
        //        await _context.SaveChangesAsync();
        //        // Xóa OTP sau khi xác nhận thành công
        //        _context.Otps.Remove(otpSession);
        //        await _context.SaveChangesAsync();

        //        // Gửi email thông tin đặt bàn
        //        var subject = "Thông Tin Đặt Bàn Thành Công";
        //        var body = $"<!DOCTYPE html>\r\n<html lang=\"vi\">\r\n<head>\r\n  " +
        //            $"  <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" " +
        //            $"content=\"width=device-width, initial-scale=1.0\">\r\n    <style>\r\n      " +
        //            $"  body {{\r\n            font-family: Arial, sans-serif;\r\n           " +
        //            $" background-color: #f4f4f4;\r\n            margin: 0;\r\n            padding: 0;\r\n   " +
        //            $"     }}\r\n        .container {{\r\n            max-width: 600px;\r\n            margin: 0 auto;\r\n          " +
        //            $"  padding: 20px;\r\n            background-color: #ffffff;\r\n            border-radius: 5px;\r\n          " +
        //            $"  box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);\r\n        }}\r\n        .header {{\r\n            background-color: #28a745;\r\n       " +
        //            $"     color: #ffffff;\r\n            padding: 10px 20px;\r\n            border-radius: 5px 5px 0 0;\r\n            text-align: center;\r\n    " +
        //            $"    }}\r\n        .content {{\r\n            padding: 20px;\r\n        }}\r\n        .info {{\r\n            font-size: 16px;\r\n         " +
        //            $"   color: #333333;\r\n            text-align: left;\r\n            margin: 20px 0;\r\n        }}\r\n        .footer {{\r\n        " +
        //            $"    font-size: 12px;\r\n            color: #888888;\r\n            text-align: center;\r\n            margin-top: 20px;\r\n        }}\r\n        .footer a {{\r\n           " +
        //            $" color: #28a745;\r\n            text-decoration: none;\r\n        }}\r\n    </style>\r\n</head>\r\n<body>\r\n    <div class=\"container\">\r\n        <div class=\"header\">\r\n   " +
        //            $"         <h1>Thông Tin Đặt Bàn</h1>\r\n        </div>\r\n        <div class=\"content\">\r\n         " +
        //            $"   <p>Chào {datBan.HoTen},</p>\r\n           " +
        //            $" <p>Cảm ơn bạn đã đặt bàn với chúng tôi. Dưới đây là thông tin đặt bàn của bạn:</p>\r\n          " +
        //            $"  <div class=\"info\">\r\n            <p><strong>Email:</strong> {datBan.Email}</p>\r\n            " +
        //            $"  <p><strong>Mật khẩu:</strong> 123456</p>\r\n            " +
        //            $"  <p><strong>Thông Tin Đặt Bàn:</strong></p>\r\n            " +
        //            $"  <p><strong>Khu Vuc</strong> {datBan.KhuvucId}</p>\r\n            " +
        //            $"  <p><strong>Ngày Đặt:</strong> {datBan.ThoiGianDat}</p>\r\n            " +
        //            $"  <p><strong>Số Người:</strong> {datBan.SoNguoi}</p>\r\n            " +
        //            $"  <p><strong>Ghi Chú:</strong> {datBan.GhiChu}</p>\r\n            " +
        //            $"  <p><strong>Phương Thức Đặt:</strong> {datBan.PhuongThucDat}</p>\r\n          " +
        //            $"  </div>\r\n        " +
        //            $"<p>Chúng tôi rất mong được đón tiếp bạn tại nhà hàng.</p>\r\n        " +
        //            $"<p>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi.</p>\r\n        " +
        //            $"</div>\r\n        <div class=\"footer\">\r\n            <p>Trân trọng,</p>\r\n            <p>Tên Nhà Hàng Của Bạn</p>\r\n         " +
        //            $"   <p>Truy cập <a href=\"https://yourwebsite.com\">website</a> của chúng tôi để biết thêm thông tin.</p>\r\n        </div>\r\n    </div>\r\n</body>\r\n</html>";

        //        var result = await _mailjetService.SendEmailAsync(datBan.Email, subject, body);
        //        if (result)
        //        {
        //            return Ok("Đặt bàn thành công, thông tin tài khoản và thông tin đặt bàn đã được gửi qua email.");
        //        }
        //        else
        //        {
        //            return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Không thể gửi email xác nhận." });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception (ex) here if needed
        //        return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Đã xảy ra lỗi khi xử lý yêu cầu." });
        //    }
        //}

        [HttpPost("confirm")]
        public async Task<IActionResult> ConfirmOtp([FromBody] OTDDatBanOnline datBan)
        {
            // Kiểm tra OTP
            var otpSession = await _context.Otps
                .Where(o => o.Email == datBan.Email && o.Otpcode == datBan.OTP)
                .SingleOrDefaultAsync();

            if (otpSession == null || otpSession.ExpiryDate < DateTime.Now)
            {
                return BadRequest(new { Message = "OTP không hợp lệ hoặc đã hết hạn." });
            }

            // Kiểm tra khách hàng có tồn tại
            var khachHang = await _context.KhachHangs
                .SingleOrDefaultAsync(kh => kh.Email == datBan.Email);

            if (khachHang == null)
            {
                // Nếu khách hàng không tồn tại, tạo mới tài khoản người dùng và khách hàng
                var user = await _context.Pusers.SingleOrDefaultAsync(u => u.Username == datBan.Email);
                if (user == null)
                {
                    user = new Puser
                    {
                        Username = otpSession.Email,
                        PasswordHash = "123456"/*BCrypt.Net.BCrypt.HashPassword("123456")*/, // Mã hóa mật khẩu 123456
                        Status = 1,
                        CreatedDate = DateTime.Now,
                        Role = 2
                    };
                    _context.Pusers.Add(user);
                    await _context.SaveChangesAsync();
                }

                // Tạo record KhachHang và lưu vào cơ sở dữ liệu
                khachHang = new KhachHang
                {
                    UserId = user.UserId,
                    HoTen = datBan.HoTen,
                    SoDienThoai = datBan.SoDienThoai,
                    Email = datBan.Email
                };
                _context.KhachHangs.Add(khachHang);
                await _context.SaveChangesAsync(); // Lưu KhachHang và lấy KhachHangId
            }

            // Lấy KhachHangId
            var khachHangId = khachHang.KhachHangId;
            string qrCodeBase64 = string.Empty;
            // Lưu thông tin đặt bàn
            try
            {
                // Tạo model DatBan
                var datBanModel = new DatBan
                {
                    KhachHangId = khachHangId,
                    KhuVucId = datBan.KhuvucId,
                    NgayDat = datBan.NgayDat,
                    ThoiGianDat = datBan.ThoiGianDat,
                    SoNguoi = datBan.SoNguoi,
                    GhiChu = datBan.GhiChu,
                    PhuongThucDat = "Online",
                    TrangThai = "Chờ duyệt", // Trạng thái chờ xác nhận
                    MaQr = null // Khởi tạo với giá trị null
                };

                // Lưu đặt bàn vào cơ sở dữ liệu
                _context.DatBans.Add(datBanModel);
                await _context.SaveChangesAsync();

                // Tạo mã QR chứa ID đặt bàn
                var qrCodeGenerator = new QRCodeGenerator();
                var qrCodeData = qrCodeGenerator.CreateQrCode($"Đặt bàn ID: {datBanModel.DatBanId}", QRCodeGenerator.ECCLevel.Q);
                var qrCode = new QRCode(qrCodeData);

                using (var bitmap = qrCode.GetGraphic(20))
                {
                    using (var stream = new MemoryStream())
                    {
                        bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                        qrCodeBase64 = Convert.ToBase64String(stream.ToArray());

                        // Cập nhật MaQr trong model
                        datBanModel.MaQr = qrCodeBase64;
                        _context.DatBans.Update(datBanModel);
                        await _context.SaveChangesAsync();
                    }
                }

                // Tạo đơn hàng
                var donHang = new DonHang
                {
                    DatBanId = datBanModel.DatBanId,
                    NhanVienId = null, // Hoặc ID của nhân viên nào đó nếu cần
                    NgayDat = DateOnly.FromDateTime(DateTime.Now),
                    TongTien = 0, // Tính tổng tiền sau
                    TrangThai = "Đang chờ xử lý",
                    ChiTietDonHangs = new List<ChiTietDonHang>()
                };

                // Kiểm tra xem có đặt món không
                if (datBan.MonAnDat != null && datBan.MonAnDat.Any())
                {
                    datBanModel.CoDatMon = true; // Set CoDatMon = true
                    foreach (var monAn in datBan.MonAnDat)
                    {
                        var chiTietDonHang = new ChiTietDonHang
                        {
                            MonAnId = monAn.MonAnId,
                            SoLuong = monAn.SoLuong,
                            Gia = monAn.Gia,
                            TrangThai = "Chờ chế biến" // Trạng thái của món ăn
                        };

                        // Thêm chi tiết đơn hàng vào danh sách
                        donHang.ChiTietDonHangs.Add(chiTietDonHang);
                    }
                    // Tính tổng tiền cho đơn hàng
                    donHang.TongTien = donHang.ChiTietDonHangs.Sum(x => x.Gia * x.SoLuong);
                }
                else
                {
                    datBanModel.CoDatMon = false; // Set CoDatMon = false nếu không có món
                }

                // Thêm đơn hàng vào cơ sở dữ liệu
                _context.DonHangs.Add(donHang);

                // Xóa OTP sau khi xác nhận thành công
                _context.Otps.Remove(otpSession);
                await _context.SaveChangesAsync();

                // Gửi email thông tin đặt bàn
                var subject = "Thông Tin Đặt Bàn Thành Công";
                var body = $"<!DOCTYPE html>\r\n<html lang=\"vi\">\r\n<head>\r\n  " +
                    $"  <meta charset=\"UTF-8\">\r\n    <meta name=\"viewport\" " +
                    $"content=\"width=device-width, initial-scale=1.0\">\r\n    <style>\r\n      " +
                    $"  body {{\r\n            font-family: Arial, sans-serif;\r\n           " +
                    $" background-color: #f4f4f4;\r\n            margin: 0;\r\n            padding: 0;\r\n   " +
                    $"     }}\r\n        .container {{\r\n            max-width: 600px;\r\n            margin: 0 auto;\r\n          " +
                    $"  padding: 20px;\r\n            background-color: #ffffff;\r\n            border-radius: 5px;\r\n          " +
                    $"  box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);\r\n        }}\r\n        .header {{\r\n            background-color: #28a745;\r\n       " +
                    $"     color: #ffffff;\r\n            padding: 10px 20px;\r\n            border-radius: 5px 5px 0 0;\r\n            text-align: center;\r\n    " +
                    $"    }}\r\n        .content {{\r\n            padding: 20px;\r\n        }}\r\n        .info {{\r\n            font-size: 16px;\r\n         " +
                    $"   color: #333333;\r\n            text-align: left;\r\n            margin: 20px 0;\r\n        }}\r\n        .footer {{\r\n        " +
                    $"    font-size: 12px;\r\n            color: #888888;\r\n            text-align: center;\r\n            margin-top: 20px;\r\n        }}\r\n        .footer a {{\r\n           " +
                    $" color: #28a745;\r\n            text-decoration: none;\r\n        }}\r\n    </style>\r\n</head>\r\n<body>\r\n    <div class=\"container\">\r\n        <div class=\"header\">\r\n   " +
                    $"         <h1>Thông Tin Đặt Bàn</h1>\r\n        </div>\r\n        <div class=\"content\">\r\n         " +
                    $"   <p>Chào {datBan.HoTen},</p>\r\n           " +
                    $" <p>Cảm ơn bạn đã đặt bàn với chúng tôi. Dưới đây là thông tin đặt bàn của bạn:</p>\r\n          " +
                    $"  <div class=\"info\">\r\n            <p><strong>Email:</strong> {datBan.Email}</p>\r\n            " +
                    $"  <p><strong>Mật khẩu:</strong> 123456</p>\r\n            " +
                    $"  <p><strong>Thông Tin Đặt Bàn:</strong></p>\r\n            " +
                    $"  <p><strong>Khu Vuc:</strong> {datBan.KhuvucId}</p>\r\n            " +
                    $"  <p><strong>Ngày Đặt:</strong> {datBan.ThoiGianDat}</p>\r\n            " +
                    $"  <p><strong>Số Người:</strong> {datBan.SoNguoi}</p>\r\n            " +
                    $"  <p><strong>Ghi Chú:</strong> {datBan.GhiChu}</p>\r\n            " +
                    $"  <p><strong>Phương Thức Đặt:</strong> {datBan.PhuongThucDat}</p>\r\n            " +
                    $"  <p><strong>Mã QR:</strong></p>\r\n            <img src=\"data:image/png;base64,{qrCodeBase64}\" alt=\"QR Code\" />\r\n        " +
                    $"  </div>\r\n        </div>\r\n        <div class=\"footer\">\r\n            <p>Nếu bạn có bất kỳ câu hỏi nào, vui lòng liên hệ với chúng tôi.</p>\r\n          " +
                    $"  <p>Trân trọng,<br>Nhà hàng Lotus</p>\r\n        </div>\r\n    </div>\r\n</body>\r\n</html>";

                await _mailjetService.SendEmailAsync(datBan.Email, subject, body); // Gửi email thông tin đặt bàn

                return Ok(new { Message = "Đặt bàn thành công!", DatBanId = datBanModel.DatBanId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi trong quá trình đặt bàn.", Error = ex.Message });
            }
        }


        public class OTDDatBanOnline
        {
          
            public string? HoTen { get; set; }
            public string? OTP { get; set; }
            public DateTime? ThoiGianDat { get; set; }
           
            public int? SoNguoi { get; set; }

            public string? GhiChu { get; set; }

            public string? PhuongThucDat { get; set; }

            public string? Email { get; set; }

            public string? SoDienThoai { get; set; }

            public DateOnly? NgayDat { get; set; }

            public int? NhanVienMoBanId { get; set; }
            public int? KhuvucId { get; set; }

            public bool? CoDatMon { get; set; }
            public List<ChiTietDonHangModelView>? MonAnDat { get; set; }


        }

        public class OTDDatBanOffline
        {
      
            public DateTime? ThoiGianDat { get; set; }
            public int? BanId { get; set; }
            public string? HoTen { get; set; }
            public int? SoNguoi { get; set; }
            public string Email { get; set; }
            public string? GhiChu { get; set; }
            public string SoDienThoai { get; set; }

            public DateOnly? NgayDat { get; set; }
            public int? NhanVienMoBanId { get; set; }
            public int? KhuvucId { get; set; }
        }
        [HttpPost("DatbanOffline")]
        public async Task<ActionResult<ResponeMessage>> DatBanOffline(OTDDatBanOffline datBanOffline)
        {
            // Kiểm tra dữ liệu đầu vào
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra xem khách hàng đã tồn tại chưa
            var khachHang = await _context.KhachHangs
                .FirstOrDefaultAsync(kh => kh.SoDienThoai == datBanOffline.SoDienThoai || kh.Email == datBanOffline.Email);

            // Nếu khách hàng chưa tồn tại, tạo mới khách hàng
            if (khachHang == null)
            {
                khachHang = new KhachHang
                {
                    HoTen = datBanOffline.HoTen,
                    SoDienThoai = datBanOffline.SoDienThoai,
                    Email = datBanOffline.Email
                };

                // Lưu khách hàng vào cơ sở dữ liệu
                _context.KhachHangs.Add(khachHang);
                await _context.SaveChangesAsync(); // Lưu thay đổi để lấy KhachHangId
            }
            
            // Kiểm tra xem ngày đặt có hợp lệ không
            if (datBanOffline.ThoiGianDat.HasValue)
            {
                var thoiGianKhachHangDat = datBanOffline.ThoiGianDat.Value; // Thời gian khách hàng muốn đặt
                var ngayKhachHangDat = thoiGianKhachHangDat.Date; // Lấy ngày từ thời gian đặt


                // Kiểm tra xem có bàn đã được đặt cho ngày và BanId không

                // Kiểm tra xem có bàn đã được đặt cho BanId không
                var banDaDat = await _context.DatBans
                    .Where(db => db.BanId == datBanOffline.BanId && db.ThoiGianDat.Value.Date == ngayKhachHangDat)
                    .ToListAsync();
              

                foreach (var datBanr in banDaDat)
                {
                    // So sánh thời gian đặt với thời gian đã có trong bảng
                    var thoiGianDaDat = datBanr.ThoiGianDat.Value;
                    var minTime = thoiGianDaDat.AddHours(-3); // 3 tiếng trước
                    var maxTime = thoiGianDaDat.AddHours(3);  // 3 tiếng sau

                    if (thoiGianKhachHangDat >= minTime && thoiGianKhachHangDat <= maxTime)
                    {
                        return BadRequest(new { isSuccess = false, message = "Bàn này đã được đặt trong khoảng thời gian không cho phép." });
                    }
                }
            }
            else
            {
                return BadRequest(new { isSuccess = false, message = "Thời gian đặt không hợp lệ." });
            }
            var ngayDat = DateOnly.FromDateTime(DateTime.Now);

            // Tạo đối tượng DatBan mới
            var datBan = new DatBan
            {
                BanId = datBanOffline.BanId,
                KhachHangId = khachHang.KhachHangId, // Sử dụng KhachHangId từ khách hàng mới hoặc đã tồn tại
                ThoiGianDat = datBanOffline.ThoiGianDat, // Thời gian khách hàng muốn đặt
                SoNguoi = datBanOffline.SoNguoi,
                GhiChu = datBanOffline.GhiChu,
                PhuongThucDat = "Offline",
                NgayDat = ngayDat, // Ngày khách hàng muốn đặt
                NhanVienMoBanId = datBanOffline.NhanVienMoBanId,
                KhuVucId = datBanOffline.KhuvucId
            };

            // Lưu đặt bàn vào cơ sở dữ liệu
            _context.DatBans.Add(datBan);
            await _context.SaveChangesAsync();

            return Ok(new { isSuccess = true });
        }
        public class MoBanofflineOTD
        {

      
            public int? BanId { get; set; }
            public int? SoNguoi { get; set; }
          
            public string? GhiChu { get; set; }
 
            public int? NhanVienMoBanId { get; set; }
            public int? KhuvucId { get; set; }
        }
        [HttpPost("MoBanoffline")]
        public async Task<ActionResult<ResponeMessage>> MoBanoffline(MoBanofflineOTD mobanoffline)
        {
            // Kiểm tra dữ liệu đầu vào
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

          
            // Nếu khách hàng chưa tồn tại, tạo mới khách hàng
         
              var  khachHang = new KhachHang
                {
                    HoTen = "Khách Hàng Vãng Lai",
                  
                };

                // Lưu khách hàng vào cơ sở dữ liệu
                _context.KhachHangs.Add(khachHang);
                await _context.SaveChangesAsync(); // Lưu thay đổi để lấy KhachHangId

          
      
            var ngayDat = DateOnly.FromDateTime(DateTime.Now);

            // Tạo đối tượng DatBan mới
            var datBan = new DatBan
            {
                BanId = mobanoffline.BanId,
                KhachHangId = khachHang.KhachHangId, // Sử dụng KhachHangId từ khách hàng mới hoặc đã tồn tại
                ThoiGianDat = DateTime.Now, // Thời gian khách hàng muốn đặt
                SoNguoi = mobanoffline.SoNguoi,
                GhiChu = mobanoffline.GhiChu,
                PhuongThucDat = "Offline",
                NgayDat = DateOnly.FromDateTime(DateTime.Now),
                NhanVienMoBanId = mobanoffline.NhanVienMoBanId,
                KhuVucId = mobanoffline.KhuvucId
            };

            // Lưu đặt bàn vào cơ sở dữ liệu
            _context.DatBans.Add(datBan);
            await _context.SaveChangesAsync();

            return Ok(new { isSuccess = true });
        }





        //[HttpPost("ThemDatBan")]
        //public async Task<ActionResult<ResponeMessage>> ADDatBan([FromBody] Chitietdatban chitietdatban)
        //{
        //    var otpEntity = await _context.Otps
        //    .Where(o => o.Email == chitietdatban.Email && o.Otpcode == chitietdatban.OTP && o.ExpiryDate > DateTime.UtcNow)
        //    .FirstOrDefaultAsync();

        //    if (otpEntity == null)
        //    {
        //        return BadRequest("Mã OTP không hợp lệ hoặc đã hết hạn.");
        //    }
        //    var KhachHangid = new SqlParameter("@KhachHangID", chitietdatban.KhachHangId ?? (object)DBNull.Value);
        //    var banID = new SqlParameter("@BanID", chitietdatban.BanId ?? (object)DBNull.Value);
        //    var NgayDat = new SqlParameter("@NgayDat", chitietdatban.NgayDat ?? (object)DBNull.Value);
        //    var thoigiandat = new SqlParameter("@ThoiGianDat", chitietdatban.ThoiGianDat ?? (object)DBNull.Value);
        //    var songuoi = new SqlParameter("@SoNguoi", chitietdatban.SoNguoi ?? (object)DBNull.Value);
        //    var Ghichu = new SqlParameter("@GhiChu", chitietdatban.GhiChu ?? (object)DBNull.Value);
        //    var phuongthucdat = new SqlParameter("@PhuongThucDat", chitietdatban.PhuongThucDat ?? (object)DBNull.Value);
        //    var NhanVienID = new SqlParameter("@NhanVienID", chitietdatban.NhanVienMoBanId ?? (object)DBNull.Value);

        //    var TrangThaiXacNhan = new SqlParameter("@TrangThaiXacNhan", SqlDbType.NVarChar, 50)
        //    {
        //        Direction = ParameterDirection.Output
        //    };
        //    var errorMessageParam = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 255)
        //    {
        //        Direction = ParameterDirection.Output
        //    };

        //    try
        //    {
        //        await _context.Database.ExecuteSqlRawAsync(
        //            "EXEC XulyDatBan @KhachHangID, @BanID, @NgayDat, @ThoiGianDat, @SoNguoi, @GhiChu, @PhuongThucDat, @NhanVienID, @TrangThaiXacNhan OUTPUT, @ErrorMessage OUTPUT",
        //            KhachHangid, banID, NgayDat, thoigiandat, songuoi, Ghichu, phuongthucdat, NhanVienID, TrangThaiXacNhan, errorMessageParam
        //        );

        //        var errorMessage = errorMessageParam.Value == DBNull.Value ? null : (string)errorMessageParam.Value;
        //        if (!string.IsNullOrEmpty(errorMessage))
        //        {
        //            return BadRequest(errorMessage);
        //        }

        //        // Create a response object or return success message


        //        return  await ReturnMessagesucces(chitietdatban);
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception (ex) here if needed
        //        return StatusCode(StatusCodes.Status500InternalServerError, "Đã xảy ra lỗi khi cập nhật thông tin.");
        //    }
        //}

        [HttpGet("bannvpv")]
        public async Task<ActionResult<ResponeMessage>> GetBansall()
        {
            // Lấy danh sách bàn từ cơ sở dữ liệu
            var result = await _context.VBanTrangThaiHomNays
        .Join(
            _context.Bans,
            vBan => vBan.BanId,
            ban => ban.BanId,
            (vBan, ban) => new { vBan, ban }
        )
        .Join(
            _context.KhuVucs,
             vBan => vBan.vBan.KhuVucId,
            khuVuc => khuVuc.KhuVucId,
            (vb, khuVuc) => new V_TrangthaibanhomnayModelView
            {
                BanId = vb.vBan.BanId,
                TrangThai = vb.vBan.TrangThai,
                NgayGioDat = vb.vBan.NgayGioDat,
                KhachHangId = vb.vBan.KhachHangId,
                DatBanId = vb.vBan.DatBanId,
                DonHangId = vb.vBan.DonHangId, // Thêm DonHangId
                Ban = new BanModelView
                {
                    BanId = vb.ban.BanId,
                    SoBan = vb.ban.SoBan,
                    KhuVucId = vb.ban.KhuVucId,
                    SoGhe = vb.ban.SoGhe,
                    ThoiGianCapNhat = vb.ban.ThoiGianCapNhat,
                    ThoiGianTao = vb.ban.ThoiGianTao
                },
                KhuVuc = new KhuVucModelView
                {
                    KhuVucId = khuVuc.KhuVucId,
                    TenKhuVuc = khuVuc.TenKhuVuc // Giả định rằng bạn có thuộc tính này
                }
            })
        .ToListAsync();


            return await ReturnMessagesucces(result);
        }


        [HttpPost("Datmonoffline")]
        public async Task<ActionResult<ResponeMessage>> DatMonOffline([FromBody] DonHangModelView donHangView)
        {
            if (donHangView == null || donHangView.ChiTietDonHangs == null || !donHangView.ChiTietDonHangs.Any())
            {
                return BadRequest(new ResponeMessage
                {
                    IsSuccess = false,
                    Message = "Thông tin đặt món không hợp lệ."
                });
            }

            // Tìm đơn hàng trong cơ sở dữ liệu
            var donHang = await _context.DonHangs.FindAsync(donHangView.DonHangId);

            // Nếu DonHang không tồn tại, tạo mới DonHang
            if (donHang == null)
            {
                var newDonHang = new DonHang
                {
                    NgayDat = DateOnly.FromDateTime(DateTime.Now),
                    NhanVienId = donHangView.NhanVienId,
                    KhachHangId = donHangView.KhachHangId,
                    DatBanId = donHangView.DatBanId,
                    TrangThai = "Đang Đặt Món",
                    TongTien = 0, // Tổng tiền sẽ được tính sau khi thêm chi tiết đơn hàng
                    NgayCapNhat = DateTime.Now
                };

                _context.DonHangs.Add(newDonHang);
                await _context.SaveChangesAsync(); // Lưu DonHang mới và lấy DonHangId

                // Gán DonHangId mới tạo cho ChiTietDonHang
                donHangView.DonHangId = newDonHang.DonHangId;

                // Lưu lại đối tượng DonHang vừa tạo
                donHang = newDonHang;
            }

            decimal tongTien = donHang.TongTien ?? 0; // Lấy tổng tiền hiện tại

            // Thêm chi tiết đơn hàng (ChiTietDonHang)
            foreach (var chiTietDonHangView in donHangView.ChiTietDonHangs)
            {
                var chiTietDonHang = new ChiTietDonHang
                {
                    DonHangId = donHang.DonHangId,  // Sử dụng DonHangId từ đối tượng donHang
                    MonAnId = chiTietDonHangView.MonAnId,
                    SoLuong = chiTietDonHangView.SoLuong,
                    Gia = chiTietDonHangView.Gia,
                    TrangThai = "Đặt Món"
                };

                tongTien += (chiTietDonHang.Gia ?? 0) * (chiTietDonHang.SoLuong ?? 0); // Cộng thêm tổng tiền chi tiết mới

                _context.ChiTietDonHangs.Add(chiTietDonHang);
            }

            // Cập nhật tổng tiền cho đơn hàng
            donHang.TongTien = tongTien;
            donHang.NgayCapNhat = DateTime.Now;

            // Cập nhật DonHang trong cơ sở dữ liệu
            _context.DonHangs.Update(donHang);

            // Lưu tất cả thay đổi
            await _context.SaveChangesAsync();
            return await ReturnMessagesucces(donHang.DonHangId);
        }
        [HttpGet("LichSuDonHang/{khachHangId}")]
        public async Task<ActionResult<ResponeMessage>> XemLichSuDonHang(int khachHangId)
        {
            // Kiểm tra xem khách hàng có tồn tại không
            var donHangs = await _context.DonHangs
                .Where(d => d.KhachHangId == khachHangId) // Giả sử bạn có trường KhachHangId trong DonHang
                .Include(d => d.ChiTietDonHangs) // Kết hợp với ChiTietDonHang
                .ToListAsync();

            if (donHangs == null || donHangs.Count == 0)
            {
                return Ok(new ResponeMessage
                {
                    IsSuccess = false,
                    Message = "Không tìm thấy đơn hàng nào.",
                    Data = null
                });
            }

            var danhSachDonHang = donHangs.Select(donHang => new
            {
                DonHangId = donHang.DonHangId,
                NgayDat = donHang.NgayDat,
                TongTien = donHang.TongTien,
                ChiTietDonHangs = donHang.ChiTietDonHangs.Select(chiTiet => new
                {
                    MonAnId = chiTiet.MonAnId,
                    TenMonAn = chiTiet.MonAn.TenMonAn, // Giả sử bạn có thuộc tính TenMonAn trong ChiTietDonHang
                    SoLuong = chiTiet.SoLuong,
                    Gia = chiTiet.Gia
                }).ToList()
            }).ToList();

            return Ok(new ResponeMessage
            {
                IsSuccess = true,
                Message = "Lịch sử đơn hàng đã được lấy thành công.",
                Data = danhSachDonHang
            });
        }
        public class Dohangid
        {
            public int? DonHangId { get; set; }
        }
        [HttpPost("GetMonAnDaDat")]
        public async Task<ActionResult<ResponeMessage>> GetMonAnDaDat(Dohangid dohangid)
        {
            // Kiểm tra xem DonHangId có tồn tại trong cơ sở dữ liệu không
            var donHang = await _context.DonHangs.FindAsync(dohangid.DonHangId);

            if (donHang == null)
            {
                return NotFound(new ResponeMessage
                {
                    IsSuccess = false,
                    Message = "Đơn hàng không tồn tại.",
                    Data = null
                });
            }

            // Lấy các món ăn đã đặt theo DonHangId, kèm theo thông tin từ bảng MonAn
            var monAnDaDat = await _context.ChiTietDonHangs
                .Where(ct => ct.DonHangId == dohangid.DonHangId)
                .Join(_context.MonAns,
                      ct => ct.ChiTietDonHangId,
                      ma => ma.MonAnId,
                      (ct, ma) => new
                      {
                          ma.TenMonAn,            // Tên món ăn
                          ct.SoLuong,             // Số lượng
                          ma.Gia,                 // Giá món ăn
                          ma.HinhAnh,     // Hình đại diện
                          ct.TrangThai            // Trạng thái
                      })
                .ToListAsync();

            // Tính tổng tiền của đơn hàng
            var tongTien = donHang.TongTien;

            // Tạo đối tượng kết quả trả về
            var ketQua = new
            {
                DonHangId = dohangid.DonHangId,
                TongTien = tongTien,
                MonAnDaDat = monAnDaDat
            };

            // Sử dụng phương thức ReturnMessagesucces để trả về kết quả
            return await ReturnMessagesucces(ketQua);
        }

        [HttpPut("{id}")]
            public async Task<ActionResult<ResponeMessage>> UpdateDatBan(int id, [FromBody] DatBanModelView datBanModelView)
            {
            if (id != datBanModelView.DatBanId)
            {
                return BadRequest("ID mismatch.");
            }
            var DatBanid = new SqlParameter("@DatBanID", datBanModelView.DatBanId);
            var KhachHangid = new SqlParameter("@KhachHangID", datBanModelView.KhachHangId);
            var banID = new SqlParameter("@BanID", datBanModelView.BanId);
            var thoigiandat = new SqlParameter("@ThoiGianDat", datBanModelView.ThoiGianDat);
            var songuoi = new SqlParameter("@SoNguoi", datBanModelView.SoNguoi);
            var Ghichu = new SqlParameter("@GhiChu", datBanModelView.GhiChu);
            var phuongthucdat = new SqlParameter("@PhuongThucDat", datBanModelView.PhuongThucDat);
             var NhanVienID = new SqlParameter("@NhanVienID", datBanModelView.NhanVienMoBanId); 
            var TrangThaiXacNhan = new SqlParameter("@TrangThaiXacNhan", datBanModelView.TrangThaiXacNhan);
           
            var errorMessageParam = new SqlParameter("@ErrorMessage", SqlDbType.NVarChar, 255)
            {
                Direction = ParameterDirection.Output
            };

            try
            {
                await _context.Database.ExecuteSqlRawAsync(
                    "EXEC XulyDatBanUpDate @DatBanID, @KhachHangID, @BanID, @ThoiGianDat,@SoNguoi,@GhiChu,@PhuongThucDat,@NhanVienID,@TrangThaiXacNhan, @ErrorMessage OUTPUT",
                   DatBanid,KhachHangid, banID, thoigiandat, songuoi, Ghichu, phuongthucdat, NhanVienID, TrangThaiXacNhan, errorMessageParam);

                //var errorMessage = (string)errorMessageParam.Value;
                var errorMessage = errorMessageParam.Value == DBNull.Value ? null : (string)errorMessageParam.Value;
                if (!string.IsNullOrEmpty(errorMessage))
                {
                    return BadRequest(errorMessage);
                }

                // Create a response object


                // Return success message using ReturnMessagesucces
                return await ReturnMessagesucces(errorMessage);
            }
            catch (Exception ex)
            {

                // Log the exception (ex) here if needed
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while updating user information.");
            }
        }
            private bool DatBanExists(int id)
            {
                return _context.DatBans.Any(e => e.DatBanId == id);
            }
        [HttpPost("DatMon")]
        public async Task<ActionResult> DatMon([FromBody] OrderRequest orderRequest)
        {
            if (orderRequest == null || !orderRequest.MonAnDat.Any())
            {
                return BadRequest("Danh sách món ăn rỗng.");
            }

            try
            {
                // Kiểm tra nếu DatBanId tồn tại
                var datBan = await _context.DatBans.FirstOrDefaultAsync(db => db.DatBanId == orderRequest.DatBanId);
                if (datBan == null)
                {
                    return NotFound($"Không tìm thấy thông tin đặt bàn với ID: {orderRequest.DatBanId}");
                }

                // Tạo mới đơn hàng
                var donHang = new DonHang
                {
                    DatBanId = orderRequest.DatBanId,
                    NgayDat = DateOnly.FromDateTime(DateTime.Now),
                    TongTien = 0, // Sẽ tính sau khi thêm chi tiết đơn hàng
                    TrangThai = "Đang chờ xử lý"
                };

                _context.DonHangs.Add(donHang);
                await _context.SaveChangesAsync(); // Lưu đơn hàng và lấy DonHangId

                // Tạo các bản ghi ChiTietDonHang từ danh sách món ăn
                foreach (var monAn in orderRequest.MonAnDat)
                {
                    var chiTietDonHang = new ChiTietDonHang
                    {
                        DonHangId = donHang.DonHangId,
                        ChiTietMonAnId = monAn.ChitietMonAnID,
                        SoLuong = monAn.SoLuong,
                        Gia = monAn.Gia,
                        TrangThai = "Chờ chế biến"
                    };

                    _context.ChiTietDonHangs.Add(chiTietDonHang);
                }

                // Tính tổng tiền cho đơn hàng và lưu lại
                donHang.TongTien = orderRequest.MonAnDat.Sum(item => item.Gia * item.SoLuong);
                _context.DonHangs.Update(donHang);
                await _context.SaveChangesAsync(); // Lưu lại chi tiết và tổng đơn hàng

                return Ok("Đặt món thành công!");
            }
            catch (Exception ex)
            {
                // Bắt lỗi và trả về thông báo chi tiết nếu có lỗi
                return StatusCode(500, $"Có lỗi xảy ra: {ex.InnerException?.Message ?? ex.Message}");
            }
        }
       

        [HttpGet("GetOrderHistory")]
        public async Task<IActionResult> GetOrderHistory(int tableId)
        {
            var donHangs = await _context.DonHangs
                .Where(dh => dh.DatBanId == tableId)
                .Include(dh => dh.ChiTietDonHangs)
                .ThenInclude(ct => ct.MonAn)
                .ToListAsync();

            var orderHistory = donHangs.Select(dh => new
            {
                DonHangId = dh.DonHangId,
                TongTien = dh.TongTien,
                Items = dh.ChiTietDonHangs.Select(ct => new {
                    MonAnName = ct.MonAn.TenMonAn,
                    Quantity = ct.SoLuong,
                    Price = ct.Gia
                }).ToList()
            });

            return Ok(orderHistory);
        }


        public class OrderRequest
        {
            public int DatBanId { get; set; }
            public List<ChitietMonAnRequest> MonAnDat { get; set; }
        }

        public class ChitietMonAnRequest
        {
            public int ChitietMonAnID { get; set; }
            public int SoLuong { get; set; }
            public decimal Gia { get; set; }
        }

        public class DonHangViewModel
        {
            public int? DonHangId { get; set; }
            public int DatBanId { get; set; }
            public int? KhachHangId { get; set; }
            public int? NhanVienId { get; set; }
            public decimal TongTien { get; set; }
            public DateTime NgayCapNhat { get; set; }
            public List<ChiTietDonHangViewModel> ChiTietDonHangs { get; set; }
        }

        public class ChiTietDonHangViewModel
        {
            public int? ChiTietDonHangId { get; set; }
            public int? DonHangId { get; set; }
            public int MonAnId { get; set; }
            public int SoLuong { get; set; }
            public decimal Gia { get; set; }
        }
        public class HuyDatBans
        {
            public int DatBanId { get; set; }
        }
       
        [HttpPost("HuyBanKhach")]
        public async Task<ActionResult<ResponeMessage>> HuyDatBan([FromBody] HuyDatBans datBan)
        {
            // Kiểm tra nếu DatBanId không hợp lệ
            if (datBan == null || datBan.DatBanId <= 0)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });
            }

            // Tìm đối tượng DatBan theo DatBanId
            var datBanEntity = await _context.DatBans.FindAsync(datBan.DatBanId);

            // Kiểm tra xem DatBan có tồn tại không
            if (datBanEntity == null)
            {
                return NotFound(new { message = "Không tìm thấy thông tin đặt bàn." });
            }

            // Cập nhật thời gian đóng bàn và trạng thái
            datBanEntity.ThoiGianDongBan = DateTime.Now;  // Thời gian hiện tại
            datBanEntity.TrangThai = "Hủy Bàn";  // Cập nhật trạng thái thành 'Hủy Bàn'

            // Lưu thay đổi vào cơ sở dữ liệu
            _context.DatBans.Update(datBanEntity);
            await _context.SaveChangesAsync();

            return Ok(new ResponeMessage // Cần trả về kết quả đúng kiểu ResponeMessage
            {
                IsSuccess = true,
                Message = "Hủy bàn thành công!",
                Data = datBanEntity.DatBanId // Có thể trả về DatBanId nếu cần
            });
        }
        // lấy bàn theo ngày tháng năm 
   

    }
}
