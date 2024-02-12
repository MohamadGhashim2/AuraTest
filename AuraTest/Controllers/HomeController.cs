using AuraTest.Data;
using AuraTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AuraTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var adminSettings = GetAdminSettings();
            var firstFilter = adminSettings.FirstFilter ?? "shoes";
            var secondFilter = adminSettings.SecoundFilter ?? "Shirts";
            var ThirdFilter = adminSettings.ThirdFilter ?? "Shirts";
            var ForthFilter = adminSettings.ForthFilter ?? "Shirts";
            var firstModel = await _context.Products
              .Include(p => p.Category)
              .Where(p => p.Category.CategoryName == firstFilter)
              .Take(15)
              .ToListAsync();
            var secondModel = await _context.Products
              .Include(p => p.Category)
              .Where(p => p.Category.CategoryName == secondFilter)
              .Take(15)
              .ToListAsync();
            var thirdModel = await _context.Products
              .Include(p => p.Category)
              .Where(p => p.Category.CategoryName == ThirdFilter)
              .Take(15)
              .ToListAsync();
            var forthModel = await _context.Products
               .Include(p => p.Category)
               .Where(p => p.Category.CategoryName == ForthFilter)
               .Take(15)
               .ToListAsync();

            ViewBag.FirstModel = firstModel;
            ViewBag.SecoundModel = secondModel;
            ViewBag.ThirdModel = thirdModel;
            ViewBag.ForthModel = forthModel;
            var products = await _context.Products.ToListAsync();
            return View(products);
        }
        private AdminSettings GetAdminSettings()
        {
            return _context.AdminSettings.FirstOrDefault() ?? new AdminSettings();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
