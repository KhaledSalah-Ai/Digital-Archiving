using DocFlowSystem.Application.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace DocFlowSystem.WebUI.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        private readonly DocFlowSystem.Infrastructure.Data.ApplicationDbContext _context;

        public AccountController(IAuthService authService, DocFlowSystem.Infrastructure.Data.ApplicationDbContext context)
        {
            _authService = authService;
            _context = context;
        }

        // عرض صفحة تسجيل الدخول (GET)
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // تنفيذ تسجيل الدخول (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (!ModelState.IsValid) return View(model);

            var isValid = await _authService.LoginAsync(model.Email, model.Password);
            if (!isValid)
            {
                ModelState.AddModelError(string.Empty, "البريد الإلكتروني أو كلمة المرور غير صحيحة.");
                return View(model);
            }

            // جلب بيانات المستخدم لمعرفة دوره (Role)
            var user = _context.Users.FirstOrDefault(u => u.Email == model.Email);
            if (user == null) return View(model);

            // إنشاء الـ Claims الخاصة بالمستخدم (الهوية والصلاحية)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, user.Role) // Manager أو Employee
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // تسجيل الدخول بإنشاء الـ Cookie
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            // التوجيه بحسب الصلاحية
            if (user.Role == "Manager")
            {
                return RedirectToAction("Index", "Documents");
            }

            return RedirectToAction("Index", "Documents");
        }

        // تسجيل الخروج
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
        // 1. فتح صفحة التسجيل (متاحة فقط للمدير/رئيس الفرع)
        [HttpGet]
        [Authorize(Roles = "Manager")]
        public IActionResult Register()
        {
            return View();
        }

        // 2. استقبال البيانات وحفظها (متاحة فقط للمدير/رئيس الفرع)
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authService.RegisterAsync(model.Email, model.Password, model.Role);
            if (!result)
            {
                ModelState.AddModelError(string.Empty, "البريد الإلكتروني مستخدم مسبقاً.");
                return View(model);
            }

            // بعد نجاح إضافة موظف جديد، يمكنك إبقاؤه في نفس الصفحة مع رسالة نجاح، أو توجيهه للوحة التحكم
            TempData["SuccessMessage"] = "تم إضافة المستخدم بنجاح!";
            return RedirectToAction("Register");
        }
    }


    public class RegisterViewModel
    {
        [Required(ErrorMessage = "يرجى إدخال البريد الإلكتروني")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "يرجى إدخال كلمة المرور")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "يرجى اختيار الدور والصلاحية")]
        public string Role { get; set; } = "Employee"; // القيمة الافتراضية: موظف
    }
    public class LoginViewModel
    {
        [Required(ErrorMessage = "يرجى إدخال البريد الإلكتروني")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صحيحة")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "يرجى إدخال كلمة المرور")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}