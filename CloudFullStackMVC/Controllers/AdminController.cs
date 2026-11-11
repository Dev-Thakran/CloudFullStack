using CloudFullStackMVC.Data;
using CloudFullStackMVC.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CloudFullStackMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly HttpClient _httpClient;

        public AdminController(UserManager<ApplicationUser> userManager, IHttpClientFactory httpClientFactory)
        {
            _userManager = userManager;
            _httpClient = httpClientFactory.CreateClient("SpotSafeApi");
        }

        // GET: Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.OrderBy(u => u.Email).ToListAsync();
            var model = new List<UserRolesViewModel>();

            foreach (var u in users)
            {
                model.Add(new UserRolesViewModel
                {
                    Id = u.Id,
                    Email = u.Email ?? "",
                    FullName = u.FullName,
                    DrivesElectricVehicle = u.DrivesElectricVehicle,
                    AccessibilityNeeds = u.AccessibilityNeeds,
                    Roles = (await _userManager.GetRolesAsync(u)).ToList()
                });
            }

            return View(model);
        }

        // POST: Admin/MakeAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            await _userManager.AddToRoleAsync(user, "Admin");
            TempData["Success"] = $"{user.Email} is now an Admin.";
            return RedirectToAction(nameof(Users));
        }

        // POST: Admin/RemoveAdmin
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveAdmin(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            // Don't let an admin lock themselves out
            if (user.Id == _userManager.GetUserId(User))
            {
                TempData["Error"] = "You can't remove your own Admin role.";
                return RedirectToAction(nameof(Users));
            }

            await _userManager.RemoveFromRoleAsync(user, "Admin");
            TempData["Success"] = $"{user.Email} is no longer an Admin.";
            return RedirectToAction(nameof(Users));
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var response = await _httpClient.GetAsync("api/Report/lot-revenue");
            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = "Could not load the report.";
                return View(new List<LotReportViewModel>());
            }

            string json = await response.Content.ReadAsStringAsync();
            var report = JsonConvert.DeserializeObject<List<LotReportViewModel>>(json) ?? new();
            return View(report);
        }
    }
}