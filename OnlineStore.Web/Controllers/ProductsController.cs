using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Domain.Models;
using OnlineStore.Web.Data;

namespace OnlineStore.Web.Controllers
{
    public class ProductsController : Controller
    {
        private readonly OnlineStoreContext _context;

        public ProductsController(OnlineStoreContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var onlineStoreContext = _context.Products.Include(p => p.Category);
            return View(await onlineStoreContext.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductDetail)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("Name,Price,CategoryId,ProductDetail")] Product product)
        {
            // Check if the number of products exceeds a threshold (e.g., 10000)
            int productCount = _context.Products.Count();
            if (productCount >= 10000)  // Set your own threshold
            {
                // Redirect to Index if there are too many products
                return RedirectToAction(nameof(Index));  // Redirecting to the Index page
            }

            // Validate Name
            if (string.IsNullOrEmpty(product.Name) || product.Name.Length > 255)
            {
                ModelState.AddModelError("Name", "The Name cannot be empty or longer than 255 characters.");
            }

            // Custom validation for Price
            if (product.Price > 100000)
            {
                ModelState.AddModelError("Price", "The price is too high.");
            }
            if (product.Price < 0)
            {
                ModelState.AddModelError("Price", "The price cannot be negative.");
            }

            // Walidacja CategoryId
            if (product.CategoryId <= 0 || !_context.Categories.Any(c => c.CategoryId == product.CategoryId))
            {
                ModelState.AddModelError("CategoryId", "Invalid CategoryId.");
            }

            // Walidacja ProductDetail
            if (product.ProductDetail == null)
            {
                ModelState.AddModelError("ProductDetail", "ProductDetail is required.");
            }

            if (product.ProductDetail != null)
            {
                if (product.ProductDetail.Description != null && product.ProductDetail.Description.Length > 1000)
                {
                    ModelState.AddModelError("ProductDetail.Description", "The Description cannot be longer than 1000 characters.");
                }
            }



                if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid.");
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Validation error: {error.ErrorMessage}");
                }
                ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
                return View(product);  // Return the same view with validation errors
            }

            try
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                Console.WriteLine("Product and ProductDetail saved successfully.");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception occurred: {ex.Message}");
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, contact your system administrator.");
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);  // Return the view if an error occurred
        }




        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.ProductDetail)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            if (product.ProductDetail == null)
            {
                product.ProductDetail = new ProductDetail
                {
                    ProductId = product.ProductId
                };
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,Name,Price,CategoryId,ProductDetail")] Product product)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "Name", product.CategoryId);
                return View(product);
            }

            try
            {
                _context.Entry(product).State = EntityState.Modified;

                if (product.ProductDetail != null)
                {
                    product.ProductDetail.ProductId = product.ProductId;

                    if (product.ProductDetail.ProductDetailId == 0)
                    {
                        _context.ProductDetails.Add(product.ProductDetail);
                    }
                    else
                    {
                        _context.Entry(product.ProductDetail).State = EntityState.Modified;
                    }
                }

                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductExists(product.ProductId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products
                .Include(p => p.ProductDetail)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }

            if (product.ProductDetail != null)
            {
                _context.ProductDetails.Remove(product.ProductDetail);
            }

            _context.Products.Remove(product);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                Console.WriteLine($"Error deleting product: {ex.Message}");
                return RedirectToAction(nameof(Delete), new { id, error = "Unable to delete product." });
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
