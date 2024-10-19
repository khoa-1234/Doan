using Microsoft.AspNetCore.Mvc;

namespace QLNH.Admin.Controllers
{
    public class EmployeeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
