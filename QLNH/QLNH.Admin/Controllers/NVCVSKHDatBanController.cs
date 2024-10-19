using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QLNH.Data.Models;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using QLNH.Admin.Models;
using QLNH.Admin.Service;

namespace QLNH.Admin.Controllers
{
    [Authorize(Roles = "3")]
    public class NVCVSKHDatBanController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IUserApiClient _userApiClient;

        public NVCVSKHDatBanController(IHttpClientFactory httpClientFactory, IUserApiClient userApiClient)
        {
            _httpClientFactory = httpClientFactory;
            _userApiClient = userApiClient;
        }
        /*
        public async Task<IActionResult> Index()
        {
            var bookings = await GetBookingsFromAPI();
            return View(bookings);
        }
        */
        /*
        // Fetch list of bookings directly from the API
        public async Task<List<DatBanViewModel>> GetBookingsFromAPI()
        {
            var token = User.Claims.FirstOrDefault(c => c.Type == "Token")?.Value;
            var client = _userApiClient.CreateClientWithToken(token);

            // Call the API to get booking data
            var bookingsResponse = await client.GetAsync("/api/DatBan");

            if (bookingsResponse.IsSuccessStatusCode)
            {
                var bookingsContent = await bookingsResponse.Content.ReadAsStringAsync();

                // Assuming the API now directly returns a list that includes customer information
                var bookings = JsonConvert.DeserializeObject<ApiReponse<List<DatBanViewModel>>>(bookingsContent).Data;

                return bookings ?? new List<DatBanViewModel>();
            }

            return new List<DatBanViewModel>();
        }
        */
    }
}
