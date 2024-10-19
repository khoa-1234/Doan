using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QLNH.Data.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using QLNH.Admin.Models;
using QLNH.Admin.Service;
using Microsoft.AspNetCore.Authorization;

namespace QLNH.Admin.Controllers
{
    [Authorize(Roles = "1")]

    public class QLNVController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUserApiClient _userApiClient;
        public QLNVController(IHttpClientFactory httpClientFactory,IUserApiClient userApiClient)
        {
            _httpClientFactory = httpClientFactory;
            _userApiClient = userApiClient;
        }
        public IActionResult Create()
        {
            return View(new NhanVien()); // Trả về một đối tượng mới để tạo nhân viên
        }
        public async Task<List<NhanVien>> GetNhanVien()
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            // Sử dụng ApiClientHelper để tạo HttpClient với token
            var client = _userApiClient.CreateClientWithToken(token);

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync("/api/NhanViens");
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiReponse<List<NhanVien>>>(responseContent);
                return result.Data; // Trả về dữ liệu từ trường "data"
            }
            else
            {
                return new List<NhanVien>();
            }
        }


        public async Task<IActionResult> Index()
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            // Kiểm tra nếu không có token
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "User"); // Redirect đến trang đăng nhập
            }

            var nhanViens = await GetNhanVien();
            return View(nhanViens);
        }
        [HttpPost("CreateNhanVien")]
        public async Task<IActionResult> CreateNhanVien(NhanVienView nhanVien, IFormFile HinhAnhDaiDien)
        {
            // Lấy token từ claims
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            // Sử dụng ApiClientHelper để tạo HttpClient với token
            var client = _userApiClient.CreateClientWithToken(token);

            // Sử dụng MultipartFormDataContent để gửi dữ liệu
            using (var formData = new MultipartFormDataContent())
            {
                // Thêm các thông tin nhân viên vào form
                formData.Add(new StringContent(nhanVien.HoTen ?? string.Empty), "HoTen");
                formData.Add(new StringContent(nhanVien.ChucVu ?? string.Empty), "ChucVu");
                formData.Add(new StringContent(nhanVien.NgaySinh?.ToString("yyyy-MM-dd") ?? string.Empty), "NgaySinh");
                formData.Add(new StringContent(nhanVien.SoDienThoai ?? string.Empty), "SoDienThoai");
                formData.Add(new StringContent(nhanVien.DiaChi ?? string.Empty), "DiaChi");
                formData.Add(new StringContent(nhanVien.BoPhan ?? string.Empty), "BoPhan");

                // Kiểm tra và thêm file hình ảnh (nếu có)
                if (HinhAnhDaiDien != null && HinhAnhDaiDien.Length > 0)
                {
                    var fileContent = new StreamContent(HinhAnhDaiDien.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(HinhAnhDaiDien.ContentType);
                    formData.Add(fileContent, "HinhAnhDaiDien", HinhAnhDaiDien.FileName);
                }

                // Gửi request với dữ liệu MultipartFormDataContent
                var response = await client.PostAsync("/api/NhanViens", formData);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = JsonConvert.DeserializeObject<ApiReponse<NhanVien>>(responseContent);

                    // Thêm thông báo thành công
                    TempData["SuccessMessage"] = "Nhân viên đã được thêm thành công: " + result.Data.HoTen;
                    return RedirectToAction("Index");
                }
                else
                {
                    // Thêm lỗi vào ModelState nếu xảy ra lỗi
                    ModelState.AddModelError("", "Có xảy ra lỗi khi thêm nhân viên.");
                }
            }

            // Trả lại view nếu có lỗi
            return View(nhanVien);
        }




        public async Task<IActionResult> Edit(int id)
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            // Sử dụng ApiClientHelper để tạo HttpClient với token
            var client = _userApiClient.CreateClientWithToken(token);

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync($"/api/NhanViens/{id}");
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiReponse<NhanVien>>(responseContent);
                return View(result.Data); // Trả về nhân viên để hiển thị trong form
            }

            return NotFound();
        }


        [HttpPost("EditNhanVien")]
        public async Task<IActionResult> EditNhanVien(NhanVien nhanVien)
        {
            // Kiểm tra dữ liệu hợp lệ
         

            // Lấy token từ Claims của người dùng
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            // Sử dụng ApiClientHelper để tạo HttpClient với token
            var client = _userApiClient.CreateClientWithToken(token);

            // Thêm token vào header Authorization nếu có
            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // Chuyển đối tượng nhân viên thành JSON
            var jsonContent = JsonConvert.SerializeObject(nhanVien);
            var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            // Gọi PUT request đến API, truyền NhanVienId trong URL và nội dung JSON trong body
            var response = await client.PutAsync($"/api/NhanViens/{nhanVien.NhanVienId}", httpContent);

            // Kiểm tra kết quả phản hồi
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Cập nhật thành công!";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Có xảy ra lỗi khi cập nhật");
            }

            return View(nhanVien);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            // Sử dụng ApiClientHelper để tạo HttpClient với token
            var client = _userApiClient.CreateClientWithToken(token);

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.DeleteAsync($"/api/NhanViens/{id}");
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Nhân viên đã được xóa thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa nhân viên.";
            }

            return RedirectToAction("Index");
        }





    }
}
