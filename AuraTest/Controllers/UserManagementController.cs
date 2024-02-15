using AuraTest.Migrations;
using AuraTest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuraTest.Controllers
{
    [Authorize(Roles = "ADMIN")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IWebHostEnvironment _WebHostEnvironment;

        public UserManagementController(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager,IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _WebHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            ViewBag.Roles = await _roleManager.Roles.ToListAsync();
            ViewBag.UserManager = _userManager; // Pass UserManager<ApplicationUser> to the view
            return View(users);
        }
        [HttpPost]
        public async Task<IActionResult> ChangeRole(string userId, string roleName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Remove existing roles
            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles.ToArray());

            var admin = await _userManager.GetUserAsync(User);
            LogEditAction("The Admin" +admin.FirstName +"Changed User " + user.FirstName + " " + user.LastName+"From The Role  " + userRoles+" To The role "+roleName);
            // Add new role
            await _userManager.AddToRoleAsync(user, roleName);

            return RedirectToAction("Index");
        }
        private void LogEditAction(string message)
        {
            string logDirectoryPath = Path.Combine(_WebHostEnvironment.ContentRootPath, "Logs");

            if (!Directory.Exists(logDirectoryPath))
            {
                Directory.CreateDirectory(logDirectoryPath);
            }

            string logFilePath = Path.Combine(logDirectoryPath, "edit_log.txt");

            using (StreamWriter writer = System.IO.File.AppendText(logFilePath))
            {
                writer.WriteLine(DateTime.Now.ToString() + " - " + message);
            }
        }
    }
}
