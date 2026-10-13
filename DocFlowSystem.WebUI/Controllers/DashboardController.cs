using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using DocFlowSystem.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DocFlowSystem.WebUI.Controllers
{
    [Authorize(Roles = "Manager")]
    public class DashboardController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            ViewBag.TotalFax = _context.Documents.Count();

            // العد بناءً على القيم الصحيحة للـ Enum:
            // Completed = 3
            ViewBag.CompletedFax = _context.Documents.Count(d => (int)d.Status == 3);

            // PendingAction = 2 (قيد الإجراء / قيد التنفيذ)
            ViewBag.PendingFax = _context.Documents.Count(d => (int)d.Status == 2);

            ViewBag.UsersCount = _context.Users.Count();

            return View();
        }

        // عرض الفاكسات المكتملة (الحالة 3) داخل الـ Dashboard
        public async Task<IActionResult> CompletedFaxes()
        {
            var faxes = await _context.Documents
                .Where(d => (int)d.Status == 3)
                .ToListAsync();
            return View("FilteredFaxes", faxes);
        }

        // عرض الفاكسات قيد التنفيذ (الحالة 2) داخل الـ Dashboard
        public async Task<IActionResult> PendingFaxes()
        {
            var faxes = await _context.Documents
                .Where(d => (int)d.Status == 2)
                .ToListAsync();
            return View("FilteredFaxes", faxes);
        }

        // صفحة لإدارة وعرض المستخدمين
        public async Task<IActionResult> UsersList()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        // دالة لحذف مستخدم من النظام
        [HttpPost]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(UsersList));
        }
    }
}