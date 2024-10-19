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


namespace QLNH.Admin.Controllers
{
    public class QLBanController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUserApiClient _userApiClient;

        public QLBanController(IHttpClientFactory httpClientFactory, IUserApiClient userApiClient)
        {
            _httpClientFactory = httpClientFactory;
            _userApiClient = userApiClient;
        }

        // GET: Displays all tables in a graphical format
        public async Task<IActionResult> Index()
        {
            var bans = await GetBan();
            return View(bans); // Return the view with a graphical layout
        }

        // Helper method to get tables from API
        public async Task<List<Ban>> GetBan()
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            // Sử dụng ApiClientHelper để tạo HttpClient với token
            var client = _userApiClient.CreateClientWithToken(token);

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync("/api/Ban");
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiReponse<List<Ban>>>(responseContent);
                return result.Data; // Return the data from the API
            }
            else
            {
                return new List<Ban>(); // Return an empty list if API fails
            }
        }

        // GET: Create table form
        public IActionResult Create()
        {
            return View(new Ban()); // Return a form to create a new table
        }

        [HttpPost("CreateBan")]
        public async Task<IActionResult> CreateBan(Ban ban)
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            // Sử dụng ApiClientHelper để tạo HttpClient với token
            var client = _userApiClient.CreateClientWithToken(token);

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // Serialize the `ban` object to JSON to send to the API
            var jsonContent = JsonConvert.SerializeObject(ban);
            var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            // Send the POST request to the API to create a new table (ban)
            var response = await client.PostAsync("/api/Ban", httpContent);

            if (response.IsSuccessStatusCode)
            {
                // Success, table was created. Redirect to Index to show the updated list.
                TempData["SuccessMessage"] = "Bàn đã được thêm thành công!";
                return RedirectToAction("Index");
            }
            else
            {
                // Something went wrong, return the same view with an error message
                ModelState.AddModelError("", "Có xảy ra lỗi khi thêm bàn.");
            }

            return View(ban);
        }


        // GET: Edit form for a specific table
        public async Task<IActionResult> Edit(int id)
        {
            var ban = await GetBanById(id); // Get the specific table data by ID
            if (ban == null)
            {
                return NotFound();
            }
            return View(ban); // Return the edit view with table data
        }

        // Helper method to get a specific table by ID
        private async Task<Ban> GetBanById(int id)
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;

            // Sử dụng ApiClientHelper để tạo HttpClient với token
            var client = _userApiClient.CreateClientWithToken(token);

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var response = await client.GetAsync($"/api/Ban/{id}");
            if (response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<ApiReponse<Ban>>(responseContent);
                return result.Data;
            }

            return null; // Return null if table is not found
        }

        // POST: Handle table updates
        [HttpPost("EditBan")]
        public async Task<IActionResult> EditBan(Ban ban)
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            // Sử dụng ApiClientHelper để tạo HttpClient với token
            var client = _userApiClient.CreateClientWithToken(token);

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // Serialize the `ban` object to JSON to send to the API
            var jsonContent = JsonConvert.SerializeObject(ban);
            var httpContent = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

            // Send the PUT request to update the table
            var response = await client.PutAsync($"/api/Ban/{ban.BanId}", httpContent);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Cập nhật thành công!";
                return RedirectToAction("Index");
            }
            else
            {
                ModelState.AddModelError("", "Có xảy ra lỗi khi cập nhật bàn.");
            }

            return View(ban);
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

            var response = await client.DeleteAsync($"/api/Ban/{id}");
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true, message = "Bàn đã được xóa thành công!" });
            }
            else
            {
                return Json(new { success = false, message = "Có lỗi xảy ra khi xóa bàn." });
            }
        }


    }
}
