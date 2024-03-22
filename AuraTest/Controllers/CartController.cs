using AuraTest.Data;

using AuraTest.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace AuraTest.Controllers
{
    
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        public CartController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userManager = userManager;
            _WebHostEnvironment = webHostEnvironment;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            // Retrieve the current user
            var user = await _userManager.GetUserAsync(User);
            // Find the cart associated with the current user
      
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);
            ViewBag.product = await _context.Products.ToListAsync();
            return View(cart);
        }

        public IActionResult IsSignin()
        {
            return Challenge(new AuthenticationProperties { RedirectUri = "/" });
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int productId)
        {
            // Retrieve the current user
            var user = await _userManager.GetUserAsync(User);
            // Find or create the cart associated with the current user
            var cart = await _context.Carts.Include(c=>c.CartItems).FirstOrDefaultAsync(c => c.UserId == user.Id);
            if (cart == null)
            {
                return NotFound();
            }
            var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == productId&& ci.CartId==cart.CartId);
            if (existingCartItem != null)
            {

                existingCartItem.ProductAmount++;

                var product = await _context.Products.FindAsync(productId);
                if (product != null)
                {
                    product.ProductAmount--;
                }
            }
            else
            {

                // Add the selected product to the cart
                var product = await _context.Products.FindAsync(productId);
                if (product != null)
                {
                    var cartItem = new CartItem
                    {
                        CartId = cart.CartId,
                        ProductId = product.ProductId,
                        ProductAmount = 1 // You may want to adjust this according to user input
                    };
                    _context.CartItems.Add(cartItem);
                    product.ProductAmount--;
                }
            }
            var product1 = await _context.Products.FindAsync(productId);
            LogEditAction("User " + user.FirstName + " " + user.LastName+" Added the product with ID " + product1.ProductId+" To The Cart with id "+cart.CartId);
            await _context.SaveChangesAsync();
            if (product1.ProductAmount<=0)
            {
                return RedirectToAction("Index", "Home");
            }
            return NoContent();
        }
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int itemId)
        {
            // Retrieve the current user
            var user = await _userManager.GetUserAsync(User);

            // Find the cart associated with the current user
            var cart = await _context.Carts
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.UserId == user.Id);

            if (cart == null)
            {
                return NotFound();
            }

            // Find the cart item to delete
            var cartItemToDelete = cart.CartItems.FirstOrDefault(ci => ci.ItemId == itemId);
            if (cartItemToDelete == null)
            {
                return NotFound();
            }

            // Update the product's stock by adding back the quantity of the cart item being deleted
            var product = await _context.Products.FindAsync(cartItemToDelete.ProductId);
            if (product != null)
            {
                // If the cart item's quantity is greater than one, reduce the quantity by one
                // Otherwise, remove the entire cart item from the cart
                if (cartItemToDelete.ProductAmount > 1)
                {
                    cartItemToDelete.ProductAmount--;
                    product.ProductAmount++; // Increment the product amount
                }
                else
                {
                    // If the cart item's quantity is exactly one, remove the entire cart item
                    product.ProductAmount++;
                    _context.CartItems.Remove(cartItemToDelete);
                }
            }

            // Log the deletion action
            LogEditAction($"User {user.FirstName} {user.LastName} deleted item with ID {itemId} from cart with ID {cart.CartId}");

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
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