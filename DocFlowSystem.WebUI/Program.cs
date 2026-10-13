using DocFlowSystem.Application.Interfaces;
using DocFlowSystem.Application.Services;
using DocFlowSystem.Infrastructure.Data;
using DocFlowSystem.Infrastructure.Repositories;
using DocFlowSystem.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using DocFlowSystem.Domain.Entities;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Dependency Injection
builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
builder.Services.AddScoped<IDocumentService, DocumentService>();

builder.Services.AddScoped<IAuthService, AuthService>();


// أضف هذه الخدمة لإدارة ملفات تعريف الارتباط (Cookies)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // صفحة الدخول إذا لم يكن المستخدم مسجلاً
        options.AccessDeniedPath = "/Account/AccessDenied"; // صفحة الصلاحيات المرفوضة
        options.ExpireTimeSpan = TimeSpan.FromHours(2); // مدة صلاحية الجلسة
    });



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // التأكد من عدم وجود مستخدمين مسبقاً
        if (!context.Users.Any())
        {
            var passwordHasher = new PasswordHasher<User>();
            var adminUser = new User
            {
                Email = "manager@docflow.com",
                Username = "manager@docflow.com",
                Role = "Manager"
            };
            adminUser.PasswordHash = passwordHasher.HashPassword(adminUser, "Admin123!");

            context.Users.Add(adminUser);
            context.SaveChanges();
        }
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "حدث خطأ أثناء إنشاء المستخدم الافتراضي.");
    }
}
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // <-- مهم جداً أن تسبق Authorization

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();