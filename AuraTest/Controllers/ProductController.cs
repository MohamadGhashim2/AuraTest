using Microsoft.AspNetCore.Mvc;
using AuraTest.Models;
using AuraTest.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;



namespace AuraTest.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _WebHostEnvironment;

        public ProductController(ApplicationDbContext context,IWebHostEnvironment webHostEnvironment)
        {
            _WebHostEnvironment= webHostEnvironment;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var products = await _context.Products.Include(p => p.Category).ToListAsync();
            return View(products);
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
            ViewBag.CategoryId = new SelectList(_context.Categories, "CategoryId", "CategoryName");

            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile imageFile)
        {
            string stringFileName = await UploadFile(imageFile);
            var product1 = new Product
            {
                ProductColor=product.ProductColor,
                ProductImageUrl = stringFileName,
                ProductDescription= product.ProductDescription,
                ProductId= product.ProductId,
                ProductName= product.ProductName,
                ProductPrice= product.ProductPrice,
                ProductSize= product.ProductSize,
                CategoryId= product.CategoryId,
                
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
            string stringFileName = await UploadFile(imageFile);
                   var product1 = new Product
            {
                ProductColor=product.ProductColor,
                ProductImageUrl = stringFileName,
                ProductDescription= product.ProductDescription,
                ProductId= product.ProductId,
                ProductName= product.ProductName,
                ProductPrice= product.ProductPrice,
                ProductSize= product.ProductSize,
                CategoryId= product.CategoryId,
                
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
