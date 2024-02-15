using AuraTest.Data;
using AuraTest.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace AuraTest.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _WebHostEnvironment;
        private readonly UserManager<ApplicationUser> _userManager;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment,UserManager<ApplicationUser> userManager)
        {
            _WebHostEnvironment = webHostEnvironment;
            _context = context;
            _userManager= userManager;
        }

        public async Task<IActionResult> Index(string categoryFilter, string colorFilter, string sizeFilter, int amountFilter)
        {
            IQueryable<Product> productsQuery = _context.Products.Include(x => x.Category);
            if (!string.IsNullOrEmpty(categoryFilter))
            {
                productsQuery = productsQuery.Where(x => x.Category.CategoryName == categoryFilter);
            }

            if (!string.IsNullOrEmpty(colorFilter))
            {
                productsQuery = productsQuery.Where(x => x.ProductColor == colorFilter);
            }

            if (!string.IsNullOrEmpty(sizeFilter))
            {
                productsQuery = productsQuery.Where(x => x.ProductSize == sizeFilter);
            }
            if (!string.IsNullOrEmpty(sizeFilter))
            {
                productsQuery = productsQuery.Where(x => x.ProductAmount == amountFilter);
            }

            var filteredProducts = await productsQuery.ToListAsync();


            return View(filteredProducts);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile imageFile)
        {
            string stringFileName = await UploadFile(imageFile);
            var user = await _userManager.GetUserAsync(User);
            LogEditAction("User " + user.FirstName + " " + user.LastName+" Created the product with Name " + product.ProductName);
            var product1 = new Product
            {
                ProductColor = product.ProductColor,
                ProductImageUrl = stringFileName,
                ProductDescription = product.ProductDescription,
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductPrice = product.ProductPrice,
                ProductSize = product.ProductSize,
                CategoryId = product.CategoryId,
                ProductAmount = product.ProductAmount,
                // ...
            };
            _context.Add(product1);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private async Task<string> UploadFile(IFormFile file)
        {
            string uploadDir = Path.Combine(_WebHostEnvironment.WebRootPath, "img");
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }
            string fileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            string filePath = Path.Combine(uploadDir, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            // Populate dropdown for selecting categories
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product,IFormFile imageFile)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }
            var existingProduct = await _context.Products.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);
            LogEditAction("User " + user.FirstName + " " + user.LastName+" Edited the product with Name " + existingProduct.ProductName);
            if (imageFile != null)
            {
                // Delete the old image if it exists
                if (!string.IsNullOrEmpty(existingProduct.ProductImageUrl))
                {
                    var oldImagePath = Path.Combine(_WebHostEnvironment.WebRootPath, "img", existingProduct.ProductImageUrl);

                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Upload the new image and update the product's image URL
                string newImageUrl = await UploadFile(imageFile);
                existingProduct.ProductImageUrl = newImageUrl;
            }
            existingProduct.ProductColor = product.ProductColor;
            existingProduct.ProductDescription = product.ProductDescription;
            existingProduct.ProductName = product.ProductName;
            existingProduct.ProductPrice = product.ProductPrice;
            existingProduct.ProductSize = product.ProductSize;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.ProductAmount = product.ProductAmount;
            _context.Update(existingProduct);
            await _context.SaveChangesAsync();

            if (!ProductExists(product.ProductId))
            {
                return NotFound();
            }
           
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            var user = await _userManager.GetUserAsync(User);
            LogEditAction("User " + user.FirstName + " " + user.LastName + " Deleted the product with Name " + product.ProductName);
            if (!string.IsNullOrEmpty(product.ProductImageUrl))
            {
                var imagePath = Path.Combine(_WebHostEnvironment.WebRootPath, "img", product.ProductImageUrl);

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }
            }
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
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
