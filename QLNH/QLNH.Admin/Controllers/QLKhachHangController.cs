using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QLNH.Data.Models;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using QLNH.Admin.Models;
using QLNH.Admin.Service;

namespace QLNH.Admin.Controllers
{
    public class QLKhachHangController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUserApiClient _userApiClient;

        public QLKhachHangController(IHttpClientFactory httpClientFactory, IUserApiClient userApiClient)
        {
            _httpClientFactory = httpClientFactory;
            _userApiClient = userApiClient;
        }

        // GET: Hiển thị form tạo khách hàng mới
        public IActionResult Create()
        {
            return View(new KhachHang()); // Trả về đối tượng rỗng để tạo mới
        }

        // POST: Xử lý thêm khách hàng mới
        [HttpPost("CreateKhachHang")]
        public async Task<IActionResult> CreateKhachHang(KhachHang khachHang)
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            var client = _userApiClient.CreateClientWithToken(token);

            var jsonContent = JsonConvert.SerializeObject(khachHang);
            var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/KhachHangs", httpContent);

            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiReponse<KhachHang>>(responseContent);

                TempData["SuccessMessage"] = "Khách hàng đã được thêm thành công!";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Có xảy ra lỗi khi thêm khách hàng.");
            }

            return View(khachHang);
        }

        // GET: Lấy danh sách khách hàng từ API và hiển thị
        public async Task<IActionResult> Index()
        {
            var khachHangs = await GetKhachHangs();
            return View(khachHangs);
        }

        // Helper method để lấy danh sách khách hàng từ API
        public async Task<List<KhachHang>> GetKhachHangs()
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            // Sử dụng ApiClientHelper để tạo HttpClient với token
            var client = _userApiClient.CreateClientWithToken(token);

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync("/api/KhachHangs");
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiReponse<List<KhachHang>>>(responseContent);
                return result.Data;
            }
            else
            {
                return new List<KhachHang>(); // Nếu API lỗi, trả về danh sách rỗng
            }
        }

        // GET: Hiển thị form chỉnh sửa khách hàng
        public async Task<IActionResult> Edit(int id)
        {
            var khachHang = await GetKhachHangById(id);
            if (khachHang == null)
            {
                return NotFound();
            }
            return View(khachHang);
        }

        // Helper method để lấy chi tiết khách hàng theo ID
        private async Task<KhachHang> GetKhachHangById(int id)
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            // Sử dụng ApiClientHelper để tạo HttpClient với token
            var client = _userApiClient.CreateClientWithToken(token);

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync($"/api/KhachHangs/{id}");
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiReponse<KhachHang>>(responseContent);
                return result.Data;
            }

            return null; // Trả về null nếu không tìm thấy
        }

        // POST: Cập nhật khách hàng
        [HttpPost("EditKhachHang")]
        public async Task<IActionResult> EditKhachHang(KhachHang khachHang)
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            // Sử dụng ApiClientHelper để tạo HttpClient với token
            var client = _userApiClient.CreateClientWithToken(token);

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var jsonContent = JsonConvert.SerializeObject(khachHang);
            var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/KhachHangs/{khachHang.KhachHangId}", httpContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Cập nhật thành công!";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Có xảy ra lỗi khi cập nhật khách hàng.");
            }

            return View(khachHang);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            // Sử dụng ApiClientHelper để tạo HttpClient với token
            var client = _userApiClient.CreateClientWithToken(token);

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.DeleteAsync($"/api/KhachHangs/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Khách hàng đã được xóa thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa khách hàng.";
            }

            return RedirectToAction("Index");
        }


    }
}
