using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using QLNH.Customer.Models;
using QLNH.Customer.Service;
using QLNH.Data.Models;


namespace QLNH.Customer.Controllers
{
    public class BookingController : Controller
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IConfiguration _configuration;
        public BookingController(IUserApiClient userApiClient, IConfiguration configuration)
        {
            _userApiClient = userApiClient;
            _configuration = configuration;
        }

        public IActionResult MenuPartial()
        {

            return PartialView("_Booking");
        }
        public async Task<IActionResult> GetKhuVuc()
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            // Sử dụng ApiClientHelper để tạo HttpClient với token
            var client = _userApiClient.CreateClientWithToken(token);
            var response = await client.GetAsync("/api/KhuVuc");
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiReponse<List<KhuVucModelView>>>(responseContent);
                return Json(result.Data); // Trả về dữ liệu từ trường "data"
            }
            return Json(new { success = false, message = "Failed to call the API." });
        }
        public async Task<IActionResult> GetMenuItems()
        {
           
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            // Sử dụng ApiClientHelper để tạo HttpClient với token
            var client = _userApiClient.CreateClientWithToken(token);
            var response = await client.GetAsync("/api/MonAn");
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiReponse<List<MonAn>>>(responseContent);
                ViewBag.ImageBaseUrl = _configuration["ImageBaseUrl"];
                return Json(result.Data); // Trả về dữ liệu từ trường "data"
            }
            return Json(new { success = false, message = "Failed to call the API." });
        }
        public async Task<IActionResult> Otp([FromBody] PostOtp otp)
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

           

            // Sử dụng ApiClientHelper để tạo HttpClient với token
            var client = _userApiClient.CreateClientWithToken(token);
            // Gọi API để đặt bàn
            var jsonContent = JsonConvert.SerializeObject(otp);
            var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/DatBan/send-otp", httpContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Error while booking table: " + errorContent);
                ModelState.AddModelError("", "Có lỗi xảy ra khi đặt bàn.");
                return RedirectToAction("Index");
            }
            return Ok();
        }
        public class DatBanRequest
        {
            public string? Name { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public string? Date { get; set; }
            public string? Time { get; set; }
            public int? People { get; set; }
            public string? KhuVuc { get; set; }
            public string? Ghichu { get; set; }
            public string? Otp { get; set; }
            public List<SelectedItem>? SelectedItems { get; set; }
        }

        public class SelectedItem
        {
            public int? MonAnId { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public decimal? Price { get; set; }
            public int? Quantity { get; set; }
            public string? Image { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> DatBan([FromBody] DatBanRequest confirm)
        {
            if (confirm == null)
            {
                return BadRequest("Request is null");
            }

            if (string.IsNullOrEmpty(confirm.Name) ||
                string.IsNullOrEmpty(confirm.Email) ||
                string.IsNullOrEmpty(confirm.Date) ||
                string.IsNullOrEmpty(confirm.Time) ||
                confirm.People <= 0)
            {
                return BadRequest("Missing required fields");
            }

            var datechuyen = DateOnly.Parse(confirm.Date);
            var timechuyen = TimeOnly.Parse(confirm.Time);
            var thoiGianDat = datechuyen.ToDateTime(timechuyen);

            var datbanonline = new DatBanOnlineModelView
            {
                HoTen = confirm.Name,
                Email = confirm.Email,
                SoDienThoai = confirm.Phone,
                NgayDat = datechuyen,
                ThoiGianDat = thoiGianDat,
                SoNguoi = confirm.People,
                GhiChu = confirm.Ghichu,
                OTP = confirm.Otp,
                KhuvucId = int.TryParse(confirm.KhuVuc, out var khuVucId) ? khuVucId : (int?)null,
                MonAnDat = confirm.SelectedItems?.Select(item => new ChiTietDonHangModelView
                {
                    MonAnId = item.MonAnId,
                    Gia = item.Price,
                    SoLuong = item.Quantity,
                }).ToList()
            };

            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            var client = _userApiClient.CreateClientWithToken(token);
            var jsonContent = JsonConvert.SerializeObject(datbanonline);
            var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/DatBan/confirm", httpContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Error while booking table: " + errorContent);
                ModelState.AddModelError("", "Có lỗi xảy ra khi đặt bàn.");
                return BadRequest("Có lỗi xảy ra khi đặt bàn.");
            }
            return Ok();
        }



    }
}