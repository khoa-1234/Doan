using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using QLNH.Admin.Models;
using QLNH.Admin.Service;
using Microsoft.AspNetCore.Authorization;
using QLNH.Admin.ViewModels;
using System.Text;
using QLNH.Data.Models;

namespace QLNH.Admin.Controllers
{
    [Authorize(Roles = "4")]
    public class NVPVDatBanController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUserApiClient _userApiClient;
        private readonly IConfiguration _configuration;

        public NVPVDatBanController(IHttpClientFactory httpClientFactory, IUserApiClient userApiClient, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _userApiClient = userApiClient;
            _configuration = configuration;
        }
        public async Task<IActionResult> Index(string khuVuc, string trangThai)
        {
            // Lấy danh sách tất cả các bàn
            var tables = await GetTablesFromAPI();

            // Kiểm tra và chuẩn hóa dữ liệu đầu vào (trangThai, khuVuc)
            if (!string.IsNullOrEmpty(khuVuc) && khuVuc != "Tất Cả")
            {
                // Lọc theo khu vực (so sánh không phân biệt chữ hoa chữ thường)
                tables = tables.Where(t => t.KhuVuc.TenKhuVuc.Trim().ToLower() == khuVuc.Trim().ToLower()).ToList();
            }

            if (!string.IsNullOrEmpty(trangThai) && trangThai != "Tất cả")
            {
                // Lọc theo trạng thái (so sánh không phân biệt chữ hoa chữ thường)
                tables = tables.Where(t => t.TrangThai.Trim().ToLower() == trangThai.Trim().ToLower()).ToList();
            }

            // Truyền danh sách bàn đã lọc qua View
            ViewBag.KhuVuc = khuVuc;
            ViewBag.TrangThai = trangThai;

            return View(tables);
        }
        [HttpPost]
        public async Task<IActionResult> FilterTables([FromBody] FilterRequestModel filterRequest)
        {
            try
            {
                if (filterRequest.Ngay == DateTime.MinValue)
                {
                    return BadRequest(new { message = "Ngày không hợp lệ." });
                }

                // Gọi API và lấy danh sách bàn
                var tables = await GetTablesByDate(filterRequest.Ngay);

                // Lọc khu vực dựa trên tên khu vực
                if (!string.IsNullOrEmpty(filterRequest.KhuVuc) && filterRequest.KhuVuc != "Tất Cả")
                {
                    tables = tables
                        .Where(t => t.TenKhuVuc?.Trim().ToLower() == filterRequest.KhuVuc.Trim().ToLower())
                        .ToList();
                }

                // Lọc trạng thái nếu cần
                if (!string.IsNullOrEmpty(filterRequest.TrangThai) && filterRequest.TrangThai != "Tất cả")
                {
                    tables = tables
                        .Where(t => t.TrangThai?.Trim().ToLower() == filterRequest.TrangThai.Trim().ToLower())
                        .ToList();
                }

                // Lọc số người nếu được cung cấp
                if (filterRequest.SoNguoi.HasValue)
                {
                    tables = tables
                        .Where(t => t.SoGhe >= filterRequest.SoNguoi.Value)
                        .ToList();
                }

                return Json(tables);  // Trả về danh sách đã lọc
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}\n{ex.StackTrace}");
                return BadRequest(new { message = "Có lỗi xảy ra khi lọc bàn.", error = ex.Message });
            }
        }

        public class FilterRequestModel
        {
            public string KhuVuc { get; set; } = "Tất Cả";
            public string TrangThai { get; set; } = "Tất cả";
            public DateTime Ngay { get; set; }
            public int? SoNguoi { get; set; }
        }






        public async Task<List<V_BanNhanVienPhucVu>> GetTablesFromAPI()
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            var client = _userApiClient.CreateClientWithToken(token);

            var response = await client.GetAsync("/api/DatBan/bannvpv");
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiReponse<List<V_BanNhanVienPhucVu>>>(responseContent);
                return result.Data;
            }

            // Thêm debug để kiểm tra dữ liệu nhận được từ API
            Console.WriteLine("Không thể lấy được dữ liệu từ API");
            return new List<V_BanNhanVienPhucVu>();
        }

        public async Task<List<DatBan>> GetBookingsFromAPI()
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            // Sử dụng ApiClientHelper để tạo HttpClient với token
            var client = _userApiClient.CreateClientWithToken(token);

            var response = await client.GetAsync("/api/DatBan/");
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiReponse<List<DatBan>>>(responseContent);
                return result.Data;
            }

            return new List<DatBan>();
        }
        public class MoBanofflineOTD
        {

            public string? Username { get; set; }
            public int? BanId { get; set; }
            public int? SoNguoi { get; set; }

            public string? GhiChu { get; set; }

            public int? NhanVienMoBanId { get; set; }
            public int? KhuvucId { get; set; }
        }
        [HttpPost]
        public async Task<IActionResult> BookTable([FromBody] MoBanofflineOTD moBanofflineOTD)
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            var client = _userApiClient.CreateClientWithToken(token);

            var username = moBanofflineOTD.Username;
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Không có Username.");
            }

            // Call API to get NhanVienId
            var responseNhanVienId = await client.PostAsync("/api/NhanViens/GetNhanVienIdByUserId",
                new StringContent(JsonConvert.SerializeObject(new { username }), Encoding.UTF8, "application/json"));

            if (!responseNhanVienId.IsSuccessStatusCode)
            {
                var errorContent = await responseNhanVienId.Content.ReadAsStringAsync();
                Console.WriteLine("Error getting NhanVienId: " + errorContent);
                return BadRequest("Có lỗi xảy ra khi lấy thông tin nhân viên.");
            }

            var responseContent = await responseNhanVienId.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<NhanVienViewModel>(responseContent);
            var nhanvienId = result.NhanvienId;

            // Create a new booking data object
            var newBooking = new MoBanofflineOTD
            {
                BanId = moBanofflineOTD.BanId,
                SoNguoi = moBanofflineOTD.SoNguoi,
                KhuvucId = moBanofflineOTD.KhuvucId,
                GhiChu = moBanofflineOTD.GhiChu,
                NhanVienMoBanId = nhanvienId
            };

            // Call the booking API
            var jsonContent = JsonConvert.SerializeObject(newBooking);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/DatBan/MoBanoffline", httpContent);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Error while booking table: " + errorContent);
                return BadRequest("Có lỗi xảy ra khi đặt bàn.");
            }

            // Deserialize the response from the booking API to get the DatBan object
            var responseBookingContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response content from API: " + responseBookingContent);

            var datBanResult = JsonConvert.DeserializeObject<DatBan>(responseBookingContent);
            if (datBanResult == null)
            {
                return BadRequest("Không thể lấy thông tin đặt bàn.");
            }

            // **Lấy danh sách bàn từ API bannvpv**
            var tables = await GetTablesFromAPI();  // Lấy danh sách bàn từ API

            // **Tìm bàn dựa trên BanId**
            var banDat = tables.FirstOrDefault(t => t.BanId == moBanofflineOTD.BanId);

            // **Kiểm tra nếu bàn đã đặt và lấy DatBanId**
            if (banDat != null)
            {
                // Lấy thông tin đặt bàn từ danh sách bàn (ví dụ: DatBanId)
                var datBanId = banDat.DatBanId;  // Đảm bảo bảng dữ liệu có chứa `DatBanId`
                var khachHangId = banDat.KhachHangId;  // Nếu cần, lấy thêm KhachHangId
                var donHangId = banDat.DonHangId ?? "Không có";  // Đảm bảo giá trị null được thay thế

                return Ok(new
                {
                    message = "Mở bàn thành công!",
                    datBanId = datBanId,  // Trả về ID mới của bàn từ response
                    khachHangId = khachHangId,
                    donHangId = donHangId
                });
            }
            else
            {
                return BadRequest("Không tìm thấy bàn.");
            }
        }

        public class OTDDatBanOffline
        {
            public DateTime? ThoiGianDat { get; set; }
            public int? BanId { get; set; }
            public int? SoNguoi { get; set; }
            public string? GhiChu { get; set; }
            public string? HoTen { get; set; }  // Add fields for customer details
            public string? SoDienThoai { get; set; }
            public string? Email { get; set; }
            public int? KhuvucId { get; set; }
        }

        // đặt bàn cho khách hàng 
        [HttpPost]
        public async Task<IActionResult> DatBanOffline([FromBody] OTDDatBanOffline datBanOffline)
        {
            if (datBanOffline == null || !ModelState.IsValid)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });
            }

            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized(new { message = "Không có token xác thực." });
            }

            // Create an HTTP client with the provided token
            var client = _userApiClient.CreateClientWithToken(token);

            // Serialize the data that will be sent to the API
            var jsonContent = JsonConvert.SerializeObject(datBanOffline);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Call the external API to create a booking (assuming the API path is /api/DatBan/Datbanoffline)
            var response = await client.PostAsync("/api/DatBan/Datbanoffline", httpContent);

            // Handle the response from the API
            if (response.IsSuccessStatusCode)
            {
                // Return success message to the frontend
                var result = await response.Content.ReadAsStringAsync();
                return Ok(new { message = "Đặt bàn thành công!" });
            }
            else
            {
                // In case of an error, return the error message from the API
                var errorContent = await response.Content.ReadAsStringAsync();
                return BadRequest(new { message = $"Có lỗi xảy ra khi đặt bàn: {errorContent}" });
            }
        }


        [HttpGet]
        public async Task<IActionResult> GetNhanVienIdByUsername(string username)
        {
            if (string.IsNullOrEmpty(username))
            {
                return BadRequest("Username không tồn tại.");
            }

            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            var client = _userApiClient.CreateClientWithToken(token);

            // Gọi API để lấy nhanVienId từ username
            var response = await client.PostAsync("/api/NhanViens/GetNhanVienIdByUserId",
                new StringContent(JsonConvert.SerializeObject(new { username }), Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return BadRequest($"Có lỗi xảy ra khi lấy thông tin nhân viên: {errorContent}");
            }

            var responseContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<NhanVienViewModel>(responseContent);

            return Ok(new { nhanVienId = result.NhanvienId });
        }


        public class UsernameRequest
        {
            public string Username { get; set; }
        }

        [HttpGet]
        public async Task<IActionResult> GetKhuVuc()
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            var client = _userApiClient.CreateClientWithToken(token);

            var response = await client.GetAsync("/api/Khuvuc");
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiReponse<List<KhuVuc>>>(responseContent);
                return Ok(result.Data); // Trả về danh sách khu vực dưới dạng JSON
            }

            return BadRequest(new { message = "Không thể lấy danh sách khu vực." });
        }

        public class KhuVuc
        {
            public int KhuVucId { get; set; }
            public string TenKhuVuc { get; set; }
            public string MoTa { get; set; }
            // Các trường khác nếu cần
        }

        public class HuyDatBanModel
        {
            public int DatBanId { get; set; }
        }
        public async Task<IActionResult> ReleaseTable([FromBody] HuyDatBanModel request)
        {
            Console.WriteLine("ReleaseTable được gọi với DatBanId: " + request.DatBanId);

            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            var client = _userApiClient.CreateClientWithToken(token);

            // Gửi request đến API Hủy Bàn
            var jsonContent = JsonConvert.SerializeObject(request);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"/api/DatBan/HuyBanKhach", httpContent);

            if (response.IsSuccessStatusCode)
            {
                return Ok(new { message = "Bàn đã được hủy đặt thành công!" });
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                return BadRequest(new { message = $"Có lỗi xảy ra: {error}" });
            }
        }

        public class ChuyenHuong
        {
            public string? DonHangId { get; set; } // ID của Đơn Hàng
            public int? DonHangIntId { get; set; } // ID của Đơn Hàng
            public int TableId { get; set; }  // ID của bàn
            public int DatBanId { get; set; }  // ID của DatBan
            public int? KhachHangId { get; set; }  // ID khách hàng
            public int? NhanVienId { get; set; }  // ID nhân viên
            public decimal TongTien { get; set; } // Tổng tiền của đơn hàng
            public List<DishGroupViewModel> DishGroups { get; set; }  // Nhóm món ăn
            public List<DishViewModel> Dishes { get; set; }  // Món ăn
            public List<ChiTietDonHangViewModel> ChiTietDonHangs { get; set; }  // Danh sách chi tiết đơn hàng
            public List<ThanhToanViewModel> ThanhToans { get; set; } = new List<ThanhToanViewModel>();
            public List<MonDaDatModel> OrderHistory { get; set; }  // Add this line
            public string ImageBaseUrl { get; set; }

        }
        [HttpPost]
        public async Task<IActionResult> ChuyenHuongDatMon([FromBody] ChuyenHuong orderDishViewModel)
        {
            if (orderDishViewModel == null)
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ. orderDishViewModel is null." });
            }

            // Đảm bảo rằng TableId và DatBanId được cung cấp
            if (orderDishViewModel.TableId == 0 || orderDishViewModel.DatBanId == 0)
            {
                return BadRequest(new { message = "TableId hoặc DatBanId bị thiếu hoặc không hợp lệ." });
            }

            // Check if DonHangId is "Không Có" or null and set DonHangIntId accordingly
            if (string.IsNullOrEmpty(orderDishViewModel.DonHangId) || orderDishViewModel.DonHangId.ToLower() == "không có")
            {
                orderDishViewModel.DonHangIntId = null;  // Gán null nếu chuỗi là "không có"
            }
            else if (int.TryParse(orderDishViewModel.DonHangId, out int result))
            {
                orderDishViewModel.DonHangIntId = result;  // Chuyển chuỗi thành số nguyên nếu hợp lệ
            }

            // Handle null or empty DonHangId by assigning DonHangIntId
            int? donHangIdValue = orderDishViewModel.DonHangIntId;

            // Tạo URL để chuyển hướng
            var redirectUrl = Url.Action("DatMon", "NVPVDatBan", new
            {
                tableId = orderDishViewModel.TableId,
                datBanId = orderDishViewModel.DatBanId,
                khachHangId = orderDishViewModel.KhachHangId,
                donHangId = orderDishViewModel.DonHangIntId

            });

            return Ok(redirectUrl); // Trả về URL cho JavaScript
        }


        public async Task<IActionResult> DatMon(int tableId, int datBanId, int khachHangId, int donHangId)
        {
            // Lấy dữ liệu món ăn từ API
            var monAnList = await GetDishesFromAPI(); // Lấy danh sách món ăn từ API
            var dishGroups = await GetDishGroupsFromAPI(); // Lấy danh sách nhóm món ăn từ API
            string imageBaseUrl = _configuration["ImageBaseUrl"];

            // Chuyển đổi từ MonAn sang DishViewModel
            var dishes = monAnList.Select(monAn => new DishViewModel
            {
                MonAnId = monAn.MonAnId,
                TenMonAn = monAn.TenMonAn,
                MoTa = monAn.MoTa,
                Gia = monAn.Gia ?? 0, // Sử dụng giá trị mặc định nếu 'Gia' là null
                NhomMonAnId = monAn.NhomMonAnId ?? 0,
                HinhAnh = $"{imageBaseUrl}/{monAn.HinhAnh}" // Gán đường dẫn đầy đủ cho hình ảnh
            }).ToList();

            // Gán danh sách món ăn vào nhóm món ăn tương ứng
            foreach (var group in dishGroups)
            {
                group.Dishes = dishes.Where(d => d.NhomMonAnId == group.NhomMonAnId).ToList();

                // Nếu nhóm có nhóm con, cũng gán món ăn cho nhóm con
                foreach (var childGroup in group.Children)
                {
                    childGroup.Dishes = dishes.Where(d => d.NhomMonAnId == childGroup.NhomMonAnId).ToList();
                }
            }

            // Tạo model cho view
            var model = new OrderDishViewModel
            {
                TableId = tableId,
                DatBanId = datBanId,
                KhachHangId = khachHangId,
                DonHangId = donHangId,
                DishGroups = dishGroups, // Gán danh sách nhóm món ăn vào model
                ImageBaseUrl = imageBaseUrl
            };

            return View(model);
        }






        [HttpPost]
        public async Task<IActionResult> BookOrder([FromBody] OrderDishViewModel orderDishViewModel)
        {
            if (orderDishViewModel == null || orderDishViewModel.ChiTietDonHangs == null || !orderDishViewModel.ChiTietDonHangs.Any())
            {
                return BadRequest(new { message = "Dữ liệu không hợp lệ." });
            }

            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            int? donHangId = orderDishViewModel.DonHangId == 0 ? (int?)null : orderDishViewModel.DonHangId;
            var client = _userApiClient.CreateClientWithToken(token);

            var bookOrderData = new DonHangModelView
            {
                DonHangId = donHangId,
                DatBanId = orderDishViewModel.DatBanId,
                NhanVienId = orderDishViewModel.NhanVienId,
                KhachHangId = orderDishViewModel.KhachHangId,
                ChiTietDonHangs = orderDishViewModel.ChiTietDonHangs.Select(ctdh => new ChiTietDonHangModelView
                {
                    MonAnId = ctdh.MonAnId,
                    SoLuong = ctdh.SoLuong,
                    Gia = ctdh.Gia,
                }).ToList(),
            };

            var jsonContent = JsonConvert.SerializeObject(bookOrderData);
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/DatBan/Datmonoffline", httpContent);

            var responseData = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ApiReponse<int>>(responseData);

            if (result != null && result.IsSuccess)
            {
                // Trả về danh sách món ăn, bao gồm hình ảnh
                return Ok(new { message = "Đặt món thành công!", donHangId = result.Data });
            }
            // Trả về DonHangId cùng với thông báo thành công
            // Trả về thông báo lỗi nếu không thành công
            return StatusCode((int)response.StatusCode, new { message = "Đã xảy ra lỗi trong quá trình đặt món." });
        }

        // Đối tượng để chứa dữ liệu phản hồi từ API
        public class DatBanResponse
        {
            public bool IsSuccess { get; set; }
            public string Message { get; set; }
            public DatBan Data { get; set; }
        }

        private async Task<List<MonAn>> GetDishesFromAPI()
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            var client = _userApiClient.CreateClientWithToken(token);

            var response = await client.GetAsync("/api/MonAn");

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiResponse<List<MonAn>>>(responseData);

                return result?.Data ?? new List<MonAn>();
            }
            return new List<MonAn>();
        }


        private async Task<List<DishGroupViewModel>> GetDishGroupsFromAPI()
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            var client = _userApiClient.CreateClientWithToken(token);

            var response = await client.GetAsync("/api/NhomMonAns");

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                // Giải mã trực tiếp vào kiểu dữ liệu DishGroupViewModel
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<DishGroupViewModel>>>(responseData);

              

                return apiResponse?.Data ?? new List<DishGroupViewModel>();
            }
            return new List<DishGroupViewModel>();
        }

        public class Dohangid
        {
            public int? DonHangId { get; set; }
        }
        public async Task<IActionResult> GetOrderHistory(int donHangId)
        {
            // Check user token from claims
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            var client = _userApiClient.CreateClientWithToken(token);

            // Prepare request
            var jsonContent = JsonConvert.SerializeObject(new { DonHangId = donHangId });
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/DatBan/GetMonAnDaDat", httpContent);

            if (response.IsSuccessStatusCode)
            {
                // Retrieve data from API
                var responseData = await response.Content.ReadAsStringAsync();

                // Deserialize into the wrapper class that matches the response
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<OrderHistoryResponse>>(responseData);

                // Pass the data to the view (apiResponse.Data.MonAnDaDat contains the list of dishes)
                return Ok(apiResponse);
            }

            // Return error if API call fails
            return BadRequest("Cannot retrieve order history.");
        }

        public class OrderHistoryResponse
        {
            public int DonHangId { get; set; }  // "donHangId"
            public decimal TongTien { get; set; }  // "tongTien"
            public List<MonDaDatModel> MonAnDaDat { get; set; }  // "monAnDaDat"
        }

        // This class represents individual dishes in the order.
        public class MonDaDatModel
        {
            public string TenMonAn { get; set; }
            public int SoLuong { get; set; }
            public decimal Gia { get; set; }
            public string hinhAnhDaiDien { get; set; }  // Ensure this is populated with the correct filename like 'pho.png'
            public string TrangThai { get; set; }
        }

        public HttpClient CreateClientWithToken(string token)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:7244"); // Base URL for your API
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public class ApiResponse<T>
        {
            public bool IsSuccess { get; set; }   // Represents whether the API call was successful
            public string Message { get; set; }   // Any message from the API (e.g., success or error message)
            public T Data { get; set; }           // The actual data returned by the API
        }
        public async Task<List<BanModel>> GetTablesByDate(DateTime ngay)
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            var client = _userApiClient.CreateClientWithToken(token);

            var formattedDate = ngay.ToString("yyyy-MM-ddTHH:mm:ss");
            Console.WriteLine($"Gọi API với ngày: {formattedDate}");

            var response = await client.GetAsync($"/api/Ban/timkiembantheongaythangnam?ngay={formattedDate}");
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                Console.WriteLine("Dữ liệu trả về từ API:");
                Console.WriteLine(responseContent); // In dữ liệu JSON để kiểm tra

                // Thay thế chuỗi không hợp lệ thành null (nếu có)
                responseContent = responseContent.Replace("\"Chua có\"", "null");
                responseContent = responseContent.Replace("\"Không có\"", "null");

                // Deserialize JSON thành danh sách bàn
                return JsonConvert.DeserializeObject<List<BanModel>>(responseContent);
            }

            Console.WriteLine($"Lỗi khi gọi API: {await response.Content.ReadAsStringAsync()}");
            return new List<BanModel>();
        }

        public class BanModel
        {
            public int BanId { get; set; }
            public string SoBan { get; set; }
            public int SoGhe { get; set; }
            public int KhuVucId { get; set; }  // ID của khu vực
            public string TenKhuVuc { get; set; }  // Tên khu vực
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
        }
       



    }
}


