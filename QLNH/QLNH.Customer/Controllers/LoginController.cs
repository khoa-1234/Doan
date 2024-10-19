using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using QLNH.Customer.Service;
using QLNH.Data.ViewModels;

namespace QLNH.Customer.Controllers
{
    public class LoginController : Controller
	{
		private readonly IUserApiClient _userApiClient;
		private readonly IConfiguration _configuration;
		public LoginController(IUserApiClient userApiClient, IConfiguration configuration)
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

			TempData["WelcomeMessage"] = $"Chào Khach {userName}!";
			return RedirectToAction("Index", "Home");
		}

		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Index", "Login");
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
