using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Data;
using System.Text;
using AutoMapper;

using Microsoft.Extensions.DependencyInjection;
using QLNHWebAPI.Service;
using QLNHWebAPI.VNPay;
using QLNHWebAPI.Models;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", builder =>
    {
        builder.WithOrigins("https://localhost:7071")  // Thay thế với URL frontend của bạn
               .AllowAnyMethod()   // Cho phép tất cả các phương thức HTTP (GET, POST, v.v.)
               .AllowAnyHeader()   // Cho phép tất cả các header
               .AllowCredentials();  // Cho phép gửi kèm cookie nếu cần thiết
    });
});

// Add services to the container.
builder.Services.AddDbContext<QlnhContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("QLNHDB")));


builder.Services.AddControllers();
builder.Services.AddScoped<VnPayService>();
builder.Services.AddSingleton<MailjetService>(provider =>
{
    var apiKey = builder.Configuration["Mailjet:ApiKey"];
    var apiSecret = builder.Configuration["Mailjet:ApiSecret"];
    return new MailjetService(apiKey, apiSecret);
});

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new[] { "Bearer" }
        }
    });
    c.SchemaFilter<SwaggerFileUploadSchemaFilter>();
});


var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
    });
}

app.UseStaticFiles();
app.UseHttpsRedirection();

// Thêm middleware CORS
app.UseCors("AllowSpecificOrigin");  // Áp dụng chính sách CORS

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
