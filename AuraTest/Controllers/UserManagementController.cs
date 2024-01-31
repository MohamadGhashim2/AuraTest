using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks; // Add this namespace

namespace AuraTest.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UserManagementController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager; // Change IdentityUser to IdentityRole

        public UserManagementController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            ViewBag.Roles = await _roleManager.Roles.ToListAsync();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRoles(string userId, string[] roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRoles.ToArray());
            await _userManager.AddToRolesAsync(user, roles);

            return RedirectToAction(nameof(Index));
        }




    }
}
