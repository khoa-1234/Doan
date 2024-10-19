using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QLNH.Customer.Service;
using QLNH.Data.Models;
using Microsoft.Extensions.Configuration;
using Azure;
using Newtonsoft.Json.Linq;
using QLNH.Customer.Models;
using System.Net.Http.Headers;

namespace QLNH.Customer.Controllers
{
    public class MenuController : Controller
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IConfiguration _configuration;

        public MenuController(IUserApiClient userApiClient, IConfiguration configuration)
        {
            _userApiClient = userApiClient;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            var client = _userApiClient.CreateClientWithToken(token);

            if (!string.IsNullOrEmpty(token))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var nhomMonAnResponse = await client.GetAsync("/api/NhomMonAns");
            var monAnResponse = await client.GetAsync("/api/MonAn");

            if (nhomMonAnResponse.IsSuccessStatusCode && monAnResponse.IsSuccessStatusCode)
            {
                var nhomMonAnResponseContent = await nhomMonAnResponse.Content.ReadAsStringAsync();
                var result1 = JsonConvert.DeserializeObject<ApiReponse<List<NhomMonAnViewModel>>>(nhomMonAnResponseContent);

                var monAnResponseContent = await monAnResponse.Content.ReadAsStringAsync();
                var result2 = JsonConvert.DeserializeObject<ApiReponse<List<MonAnViewModel>>>(monAnResponseContent);

                // Tạo MenuViewModel
                var menuViewModel = new MenuViewModel
                {
                    NhomMonAns = result1.Data,
                    MonAns = result2.Data
                };
                ViewBag.ImageBaseUrl = _configuration["ImageBaseUrl"];
                // Truyền dữ liệu xuống View
                return View(menuViewModel);
            }


            return View(new MenuViewModel()); // Trả về một MenuViewModel rỗng nếu không có dữ liệu
        }
    }
}
