using Microsoft.AspNetCore.Mvc;
using QLNH.Customer.Service;

namespace QLNH.Customer.Controllers
{
    public class CustomerController: Controller
    {
        private readonly IUserApiClient _userApiClient;
        public CustomerController(IUserApiClient userApiClient)
        {
            _userApiClient = userApiClient;
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
