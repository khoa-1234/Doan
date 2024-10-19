using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QLNH.Data.Models;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using QLNH.Admin.Service;
using QLNH.Admin.Models;

namespace QLNH.Admin.Controllers
{
    public class QLMonAnController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUserApiClient _userApiClient;

        public QLMonAnController(IHttpClientFactory httpClientFactory, IUserApiClient userApiClient)
        {
            _httpClientFactory = httpClientFactory;
            _userApiClient = userApiClient;
        }

        // GET: Display all food items (MonAn)
        public async Task<IActionResult> Index()
        {
            var monAns = await GetMonAns();
            return View(monAns); // Return the view with the list of food items
        }

        // Helper method to get MonAn data from API
        public async Task<List<MonAn>> GetMonAns()
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            var client = _userApiClient.CreateClientWithToken(token);

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync("/api/MonAn"); // API mới
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiReponse<List<MonAn>>>(responseContent);
                return result?.Data ?? new List<MonAn>();
            }
            else
            {
                return new List<MonAn>(); // Return an empty list if API fails
            }
        }

        // GET: Create MonAn form
        public IActionResult Create()
        {
            return View(new MonAn()); // Return a form to create a new product
        }

        [HttpPost("CreateMonAn")]
        public async Task<IActionResult> CreateMonAn(MonAn monAn)
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            var client = _userApiClient.CreateClientWithToken(token);

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var jsonContent = JsonConvert.SerializeObject(monAn);
            var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync("/api/MonAn", httpContent);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStringAsync();
                var addedProduct = JsonConvert.DeserializeObject<MonAn>(responseData);

                return Json(new { success = true, data = addedProduct });
            }
            else
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi thêm món ăn." });
            }
        }

        // GET: Edit form for a specific food item
        public async Task<IActionResult> Edit(int id)
        {
            var monAn = await GetMonAnById(id);
            if (monAn == null)
            {
                return NotFound();
            }
            return View(monAn);
        }

        // Helper method to get a specific MonAn by ID
        private async Task<MonAn> GetMonAnById(int id)
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            var client = _userApiClient.CreateClientWithToken(token);

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync($"/api/MonAn/{id}");
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiReponse<MonAn>>(responseContent);
                return result?.Data;
            }

            return null; // Return null if food item is not found
        }

        // POST: Handle food item updates
        [HttpPost("EditMonAn")]
        public async Task<IActionResult> EditMonAn(MonAn monAn)
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            var client = _userApiClient.CreateClientWithToken(token);

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var jsonContent = JsonConvert.SerializeObject(monAn);
            var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"/api/MonAn/{monAn.MonAnId}", httpContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Cập nhật thành công!";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật món ăn.");
            }

            return View(monAn);
        }

        // POST: Handle food item deletion
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            var client = _userApiClient.CreateClientWithToken(token);

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.DeleteAsync($"/api/MonAn/{id}");
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Món ăn đã được xóa thành công!" });
            }
            else
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa món ăn." });
            }
        }

        // GET: Display all food groups (NhomMonAn)
        public async Task<IActionResult> NhomMonAn()
        {
            var nhomMonAns = await GetNhomMonAns();
            return View(nhomMonAns); // Return the view with the list of food groups
        }

        // Helper method to get NhomMonAn data from API
        public async Task<List<NhomMonAn>> GetNhomMonAns()
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            var client = _userApiClient.CreateClientWithToken(token);

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync("/api/NhomMonAn");
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiReponse<List<NhomMonAn>>>(responseContent);
                return result?.Data ?? new List<NhomMonAn>();
            }
            else
            {
                return new List<NhomMonAn>(); // Return an empty list if API fails
            }
        }
    }
}
