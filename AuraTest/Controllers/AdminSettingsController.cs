using AuraTest.Data;
using AuraTest.Models;
using Microsoft.AspNetCore.Mvc;

namespace AuraTest.Controllers
{
    public class AdminSettingsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminSettingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult AdminSettings()
        {
            var adminSettings = GetAdminSettings();
            return View(adminSettings);
        }

        private AdminSettings GetAdminSettings()
        {
            // Implement the logic to retrieve admin settings from the database
            return _context.AdminSettings.FirstOrDefault() ?? new AdminSettings();
        }

        [HttpPost]
        public IActionResult AdminSettings(AdminSettings adminSettings)
        {
            // Save the updated admin settings
            SaveAdminSettings(adminSettings);

            // Redirect back to the admin settings page or any other page
            return RedirectToAction("AdminSettings");
        }

        private void SaveAdminSettings(AdminSettings adminSettings)
        {
            // Implement the logic to save admin settings to the database
            var existingSettings = _context.AdminSettings.FirstOrDefault();
            if (existingSettings != null)
            {
                existingSettings.FirstFilter = adminSettings.FirstFilter;
                existingSettings.SecoundFilter = adminSettings.SecoundFilter;
                existingSettings.ThirdFilter = adminSettings.ThirdFilter;
                existingSettings.ForthFilter = adminSettings.ForthFilter;
                _context.AdminSettings.Update(existingSettings);
            }
            else
            {
                _context.AdminSettings.Add(adminSettings);
            }

            _context.SaveChanges();
        }





    }
}