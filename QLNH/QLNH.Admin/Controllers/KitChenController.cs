using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QLNH.Admin.Service;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using QLNH.Data.ViewModels;
using static QLNH.Admin.Controllers.NVPVDatBanController;
using System.Text;

namespace QLNH.Admin.Controllers
{
    [Authorize(Roles = "5")]
    public class KitchenController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUserApiClient _userApiClient;

        public KitchenController(IHttpClientFactory httpClientFactory, IUserApiClient userApiClient)
        {
            _httpClientFactory = httpClientFactory;
            _userApiClient = userApiClient;
        }

        // Action để hiển thị danh sách đơn hàng
        public async Task<IActionResult> Index()
        {
            var donHangs = await GetDonHangFromAPI();
            return View(donHangs);
        }

        // Hàm lấy danh sách đơn hàng từ API
        private async Task<List<DonHangViewModel>> GetDonHangFromAPI()
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            var client = _userApiClient.CreateClientWithToken(token);

            var response = await client.GetAsync("/api/DonHang/vBep");

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<List<DonHangViewModel>>>(responseContent);

                if (apiResponse != null && apiResponse.IsSuccess)
                {
                    return apiResponse.Data;
                }
            }

            return new List<DonHangViewModel>();
        }

        [HttpGet]
        public async Task<IActionResult> Details([FromQuery] int donHangId)
        {
            if (donHangId <= 0)
            {
                return BadRequest("Invalid Order ID.");
            }

            Console.WriteLine($"Requesting details for DonHang ID: {donHangId}");

            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            var client = _userApiClient.CreateClientWithToken(token);

            var jsonContent = JsonConvert.SerializeObject(new { DonHangId = donHangId });
            var httpContent = new StringContent(jsonContent, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/DatBan/GetMonAnDaDat", httpContent);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse<OrderHistoryResponse>>(responseData);

                if (apiResponse != null && apiResponse.IsSuccess)
                {
                    return Json(apiResponse.Data);
                }
            }

            return BadRequest("Unable to fetch order details.");
        }


        // Lớp mô hình phản hồi từ API cho chi tiết đơn hàng
        public class OrderHistoryResponse
        {
            public int DonHangId { get; set; }  // ID của đơn hàng
            public decimal TongTien { get; set; }  // Tổng tiền của đơn hàng
            public List<MonDaDatModel> MonAnDaDat { get; set; }  // Danh sách món ăn đã đặt
        }

        // Lớp mô hình món ăn đã đặt
        public class MonDaDatModel
        {
            public string TenMonAn { get; set; }  // Tên món ăn
            public int SoLuong { get; set; }  // Số lượng món ăn
            public decimal Gia { get; set; }  // Giá của món ăn
            public string hinhAnhDaiDien { get; set; }  // Hình ảnh món ăn
            public string TrangThai { get; set; }  // Trạng thái món ăn
        }

        public class ApiResponse<T>
        {
            public bool IsSuccess { get; set; }
            public string Message { get; set; }
            public T Data { get; set; }
        }
    }
}
