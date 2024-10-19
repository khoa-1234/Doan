using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using QLNH.Admin.Service;
using QLNH.Data.ViewModels;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace QLNH.Admin.Controllers
{

    public class UserController : Controller
    {
        private readonly IUserApiClient _userApiClient;
        private readonly IConfiguration _configuration;
        public UserController(IUserApiClient userApiClient, IConfiguration configuration)
        {
            _userApiClient = userApiClient;
            _configuration = configuration;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> Login()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginReQuest request)
        {
            if (!ModelState.IsValid)
                return View(ModelState);

            var token = await _userApiClient.Authenticate(request);
            if (string.IsNullOrEmpty(token))
            {
                // Handle login failure
                ModelState.AddModelError("", "Login failed.");
                return View(ModelState);
            }

            var userPrincipal = this.ValidateToken(token);

            // Retrieve the role and username from claims
            var roleClaim = userPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            var nameClaim = userPrincipal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);

            if (roleClaim == null || nameClaim == null)
            {
                // Handle missing role or name claim
                ModelState.AddModelError("", "User role or name not found.");
                return View(ModelState);
            }

            var userRole = roleClaim.Value;
            var userName = nameClaim.Value;

            // Add token, role, and name to claims
            var claims = new List<Claim>
    {
        new Claim("Token", token),
        new Claim(ClaimTypes.Role, userRole),
        new Claim(ClaimTypes.Name, userName), // Add the user's name here

    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var authProperties = new AuthenticationProperties
            {
                ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                IsPersistent = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);

            // Set the welcome message and redirect based on the user's role
            if (userRole == "1") // Manager
            {
                TempData["WelcomeMessage"] = $"Chào mừng quản lý {userName}!";
                return RedirectToAction("Index", "Home");
            }
            else if (userRole == "2") // Employee
            {
                TempData["WelcomeMessage"] = $"Chào mừng nhân viên {userName}!";
                return RedirectToAction("Index", "Home");
            }
            else if (userRole == "3") // Customer Support Staff
            {
                TempData["WelcomeMessage"] = $"Chào mừng nhân viên chăm sóc khách hàng {userName}!";
                return RedirectToAction("Index", "Home");
            }
            else if (userRole == "4") // Waitstaff
            {
                TempData["WelcomeMessage"] = $"Chào mừng nhân viên phục vụ {userName}!";
                return RedirectToAction("Index", "Home");
            }
            else if (userRole == "5") // Kitchen Staff
            {
                TempData["WelcomeMessage"] = $"Chào mừng nhân viên bếp {userName}!";
                return RedirectToAction("Index", "Kitchen");
            }
            else
            {
                // Handle unknown role
                ModelState.AddModelError("", "Unknown role.");
                return RedirectToAction("Login", "User");
            }
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "User");
        }
        private ClaimsPrincipal ValidateToken(string jwttoken)
        {
            IdentityModelEventSource.ShowPII = true;
            SecurityToken validateToken;
            TokenValidationParameters validationParameters = new TokenValidationParameters();
            validationParameters.ValidAudience = _configuration["Jwt:Issuer"];
            validationParameters.ValidIssuer = _configuration["Jwt:Issuer"];
            validationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwttoken, validationParameters, out validateToken);
            return principal;
        }


    }
}
