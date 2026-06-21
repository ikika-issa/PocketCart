using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PocketCartApp.Domain.Identity_Models;

namespace PocketCartApp.Web.Controllers
{
    [Authorize(Roles = "Admin, Manager")]
    public class EmployeesController : Controller
    {
        private readonly UserManager<PocketCartApplicationUser> _userManager;

        public EmployeesController(UserManager<PocketCartApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task <IActionResult> Index()
        {
            var employees = _userManager.Users.ToList();

            ViewBag.UserRoles = new Dictionary<string, string>();

            foreach (var employee in employees)
            {
                var roles = await _userManager.GetRolesAsync(employee);

                ViewBag.UserRoles[employee.Id] =
                    roles.FirstOrDefault() ?? "No Role";
            }

            return View(employees);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Disable(string id)
        {
            var currentUserId = _userManager.GetUserId(User);

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            if (user.Id == currentUserId)
            {
                TempData["Error"] = "You cannot disable your own account while logged in.";
                return RedirectToAction(nameof(Index));
            }

            user.LockoutEnabled = true;
            user.LockoutEnd = DateTimeOffset.MaxValue;

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Enable(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            user.LockoutEnd = null;

            await _userManager.UpdateAsync(user);

            return RedirectToAction(nameof(Index));
        }
    }
}
