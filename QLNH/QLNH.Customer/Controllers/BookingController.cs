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
        // tìm khách hàng theo số đt 
        [HttpGet]
        public async Task<IActionResult> GetCustomerByPhone(string phoneNumber)
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            // Sử dụng ApiClientHelper để tạo HttpClient với token
            var client = _userApiClient.CreateClientWithToken(token);
            var response = await client.GetAsync($"/api/KhachHangs/Laykhachhangbangsodienthoai?sodienthoai={phoneNumber}");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiReponse<KhachHangModel>>(responseContent);
                if (result != null && result.Data != null)
                {
                    return Json(new
                    {
                        isSuccess = true,
                        data = result.Data
                    });
                }
            }

            return Json(new { isSuccess = false, message = "Không tìm thấy khách hàng" });
        }
        public class KhachHangModel
        {
            public int KhachHangId { get; set; }
            public string HoTen { get; set; }
            public string SoDienThoai { get; set; }
            public string Email { get; set; }
            public string DiaChi { get; set; }
            public int? UserId { get; set; }
            public List<DatBan> DatBans { get; set; }
            public List<DonHang> DonHangs { get; set; }
            
        }

        public class BanDetailsModel
        {
            public int BanId { get; set; }
            public int? KhuVucId { get; set; }
            public string DonHangId { get; set; }
            public string TrangThai { get; set; }
            private DateTime? _ngayGioDat;
            public DateTime? NgayGioDat
            {
                get => _ngayGioDat;
                set
                {
                    if (value == null || value.ToString().ToLower() == "chưa có")
                        _ngayGioDat = null;
                    else
                        _ngayGioDat = value;
                }
            }
            public string KhachHangId { get; set; }
            public string DatBanId { get; set; }
            public BanModel Ban { get; set; }
            public KhuVucModel KhuVuc { get; set; }
        }

        public class BanModel
        {
            public int BanId { get; set; }
            public int SoBan { get; set; }
            public int KhuVucId { get; set; }
            public int SoGhe { get; set; }
            public DateTime? ThoiGianCapNhat { get; set; }
            public DateTime? ThoiGianTao { get; set; }
        }

        public class KhuVucModel
        {
            public int KhuVucId { get; set; }
            public string TenKhuVuc { get; set; }
            public string MoTa { get; set; }
        }
        [HttpGet]
        public async Task<IActionResult> GetBanByKhuVucAndNgay(int khuVucId, string ngay, string trangThai)
        {
            // Lấy token từ claim của người dùng hiện tại
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            var client = _userApiClient.CreateClientWithToken(token);

            // Chuyển đổi ngày thành DateTime nếu có
            DateTime parsedDate;
            if (!DateTime.TryParse(ngay, out parsedDate))
            {
                return BadRequest("Ngày không hợp lệ.");
            }

            // Gọi hàm GetTablesByKhuVucAndDate để lấy danh sách bàn
            var filteredTables = await GetTablesByKhuVucAndDate(khuVucId, parsedDate);

            // Lọc trạng thái nếu cần (không lọc nếu là "Tất cả")
            if (!string.IsNullOrEmpty(trangThai) && trangThai != "Tất cả")
            {
                filteredTables = filteredTables
                    .Where(t => t.TrangThai?.Trim().ToLower() == trangThai.Trim().ToLower())
                    .ToList();
            }

            // Trả về dữ liệu bàn sau khi lọc
            return Json(filteredTables);
        }


        public async Task<List<BanDetailsModel>> GetTablesByKhuVucAndDate(int khuVucId, DateTime ngay)
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            var client = _userApiClient.CreateClientWithToken(token);

            var formattedDate = ngay.ToString("yyyy-MM-ddTHH:mm:ss");
            Console.WriteLine($"Gọi API với ngày: {formattedDate} và khu vực: {khuVucId}");

            // Gọi API để lấy thông tin các bàn theo ngày và khu vực
            var response = await client.GetAsync($"/api/Ban/timkiembantheongaythangnam?khuVucId={khuVucId}&ngay={formattedDate}");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Lỗi khi gọi API: {await response.Content.ReadAsStringAsync()}");
                return new List<BanDetailsModel>();
            }

            var responseContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Dữ liệu trả về từ API:");
            Console.WriteLine(responseContent); // In dữ liệu JSON để kiểm tra

            // Thay thế chuỗi không hợp lệ thành null (nếu có)
            responseContent = responseContent.Replace("\"Chua có\"", "null");
            responseContent = responseContent.Replace("\"Không có\"", "null");

            // Deserialize JSON thành danh sách bàn đã được lọc theo khu vực và ngày
            var filteredTables = JsonConvert.DeserializeObject<List<BanDetailsModel>>(responseContent)
                .Where(t => t.KhuVucId == khuVucId)  // Lọc theo khu vực
                .ToList();

            // Gọi API /Ban để lấy danh sách tất cả các bàn bao gồm số ghế
            var allTablesResponse = await client.GetAsync("/api/Ban");
            if (!allTablesResponse.IsSuccessStatusCode)
            {
                Console.WriteLine($"Lỗi khi gọi API /Ban: {await allTablesResponse.Content.ReadAsStringAsync()}");
                return filteredTables;  // Trả về danh sách đã lọc nếu không thể lấy được thông tin tất cả các bàn
            }

            var allTablesContent = await allTablesResponse.Content.ReadAsStringAsync();
            var allTables = JsonConvert.DeserializeObject<ApiReponse<List<BanModel>>>(allTablesContent)?.Data;

            // Kết hợp dữ liệu đã lọc với số ghế từ danh sách tất cả các bàn
            foreach (var filteredTable in filteredTables)
            {
                // Khởi tạo đối tượng Ban nếu nó là null
                if (filteredTable.Ban == null)
                {
                    filteredTable.Ban = new BanModel();
                }

                // Tìm thông tin số ghế từ danh sách tất cả các bàn
                var tableInfo = allTables?.FirstOrDefault(table => table.BanId == filteredTable.BanId);
                if (tableInfo != null)
                {
                    filteredTable.Ban.SoGhe = tableInfo.SoGhe;  // Thêm thông tin số ghế vào dữ liệu bàn
                }
            }

            return filteredTables;
        }
        public async Task<IActionResult> GetMenuWithSubGroups()
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            // Use ApiClientHelper to create HttpClient with token
            var client = _userApiClient.CreateClientWithToken(token);

            // Fetch dishes from /api/MonAn
            var monAnResponse = await client.GetAsync("/api/MonAn");
            if (!monAnResponse.IsSuccessStatusCode)
            {
                return Json(new { success = false, message = "Failed to fetch dishes from API." });
            }
            var monAnContent = await monAnResponse.Content.ReadAsStringAsync();
            var monAnResult = JsonConvert.DeserializeObject<ApiReponse<List<MonAn>>>(monAnContent);

            // Fetch menu groups from /api/NhomMonAns
            var nhomMonAnResponse = await client.GetAsync("/api/NhomMonAns");
            if (!nhomMonAnResponse.IsSuccessStatusCode)
            {
                return Json(new { success = false, message = "Failed to fetch menu groups from API." });
            }
            var nhomMonAnContent = await nhomMonAnResponse.Content.ReadAsStringAsync();
            var nhomMonAnResult = JsonConvert.DeserializeObject<ApiReponse<List<NhomMonAn>>>(nhomMonAnContent);

            // Combine dishes into the correct menu groups
            var menuItems = BuildMenuHierarchy(nhomMonAnResult.Data, monAnResult.Data);

            // Modify image URLs in the result data
            var imageBaseUrl = _configuration["ImageBaseUrl"];
            foreach (var dish in monAnResult.Data)
            {
                if (!string.IsNullOrEmpty(dish.HinhAnh))
                {
                    dish.HinhAnh = $"{imageBaseUrl}{dish.HinhAnh}";
                }
            }

            return Json(menuItems); // Return the structured menu with dishes
        }


        // Helper method to build a nested menu hierarchy with dishes included
        private List<MenuItem> BuildMenuHierarchy(List<NhomMonAn> menuGroups, List<MonAn> dishes)
        {
            // Create a dictionary to map dishes by their group ID
            var dishesByGroup = dishes.GroupBy(d => d.NhomMonAnId)
                                      .ToDictionary(g => g.Key, g => g.ToList());

            // Recursively build the menu hierarchy
            List<MenuItem> BuildGroup(List<NhomMonAn> groups)
            {
                return groups.Select(group => new MenuItem
                {
                    NhomMonAnId = group.NhomMonAnId,
                    TenNhom = group.TenNhom,
                    Children = BuildGroup(group.Children), // Recursively add sub-groups
                    Dishes = dishesByGroup.ContainsKey(group.NhomMonAnId) ? dishesByGroup[group.NhomMonAnId] : new List<MonAn>()
                }).ToList();
            }

            // Start building from top-level groups (root groups)
            return BuildGroup(menuGroups);
        }

        // Define a view model for menu items
        public class MenuItem
        {
            public int NhomMonAnId { get; set; }
            public string TenNhom { get; set; }
            public List<MenuItem> Children { get; set; } = new List<MenuItem>();
            public List<MonAn> Dishes { get; set; } = new List<MonAn>();
        }

        // Define the response models for NhomMonAn and MonAn
        public class NhomMonAn
        {
            public int NhomMonAnId { get; set; }
            public string TenNhom { get; set; }
            public List<NhomMonAn> Children { get; set; } = new List<NhomMonAn>();
        }

        public class MonAn
        {
            public int MonAnId { get; set; }
            public string TenMonAn { get; set; }
            public string MoTa { get; set; }
            public int NhomMonAnId { get; set; }
            public decimal Gia { get; set; }
            public string HinhAnh { get; set; }
        }
    }
}
