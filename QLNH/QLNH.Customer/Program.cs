

using Microsoft.AspNetCore.Authentication.Cookies;
using QLNH.Customer.Service;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddRazorRuntimeCompilation(); // Bật tính năng Runtime Compilation
// Đăng ký IHttpClientFactory
builder.Services.AddHttpClient();
// Đăng ký IHttpClientFactory
builder.Services.AddHttpClient();
builder.Services.AddTransient<IUserApiClient,UserApiClient>();
// Thêm dịch vụ cho Controllers
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
	.AddCookie(options =>
	{
		options.LoginPath = "/Login/Index";
		options.AccessDeniedPath = "/Login/Forbiden";
	});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
