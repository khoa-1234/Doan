using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using QLNH.Customer.Service;
using QLNH.Data.Models;
using QLNH.web.Models;
using System.Diagnostics;
using static QLNH.Customer.Controllers.MenuController;

namespace QLNH.web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUserApiClient _userApiClient;
        private readonly IConfiguration _configuration;
        public HomeController(ILogger<HomeController> logger,IUserApiClient userApiClient,IConfiguration configuration)
        {
            _logger = logger;
            _userApiClient = userApiClient;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
           
          
			var user = User.Identity.Name;
			return View();
        }

    

    
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
