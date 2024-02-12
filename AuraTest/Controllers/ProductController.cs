using AuraTest.Data;
using AuraTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;

namespace AuraTest.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _WebHostEnvironment;

        public ProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _WebHostEnvironment = webHostEnvironment;
            _context = context;
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
            ViewBag.Categories = new SelectList(_context.Categories, "CategoryId", "CategoryName"); return View(product);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile imageFile)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }
            string stringFileName = await UploadFile(imageFile);
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

                // ...
            };
            _context.Update(product1);
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
    }
}
