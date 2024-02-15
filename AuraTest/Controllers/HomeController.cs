using AuraTest.Data;
using AuraTest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Threading.Tasks;

namespace AuraTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        
       
        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger,UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _logger = logger;
            _userManager = userManager;
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
            var user = await _userManager.GetUserAsync(User);
            
            if (user!=null)
            {
                var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);
                ViewBag.cart = cart;

            }
           

            
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
