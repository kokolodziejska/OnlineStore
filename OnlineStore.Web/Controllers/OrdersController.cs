using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineStore.Domain.Models;
using OnlineStore.Web.Data;

namespace OnlineStore.Web.Controllers
{
    public class OrdersController : Controller
    {
        private readonly OnlineStoreContext _context;

        public OrdersController(OnlineStoreContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _context.Orders
                .Include(o => o.OrderProducts)
                .ThenInclude(op => op.Product)
                .ToListAsync();

            return View(orders);
        }

        public IActionResult Create()
        {
            ViewData["Products"] = new SelectList(_context.Products, "ProductId", "Name");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create([Bind("OrderId,CustomerName,OrderDate")] Order order, int[] productIds, int[] quantities)
        {
            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState jest nieprawidłowy. Błędy:");
                foreach (var kvp in ModelState)
                {
                    foreach (var error in kvp.Value.Errors)
                    {
                        Console.WriteLine($"- Klucz: '{kvp.Key}', błąd: {error.ErrorMessage}");
                    }
                }
            }

            Console.WriteLine($"DEBUG: order.CustomerName = '{order.CustomerName}'");
            Console.WriteLine($"DEBUG: order.OrderDate = '{order.OrderDate}'");

            if (productIds == null || quantities == null || productIds.Length != quantities.Length)
            {
                ModelState.AddModelError("", "Invalid products or quantities.");
            }

            if (productIds != null && productIds.GroupBy(p => p).Any(g => g.Count() > 1))
            {
                ModelState.AddModelError("", "Duplicate products are not allowed.");
            }

            if (productIds != null && productIds.Any(p => p == 0))
            {
                ModelState.AddModelError("", "All product selections must be valid.");
            }

            if (quantities != null && quantities.Any(q => q <= 0))
            {
                ModelState.AddModelError("", "All quantities must be greater than zero.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();

                    for (int i = 0; i < productIds.Length; i++)
                    {
                        var orderProduct = new OrderProduct
                        {
                            OrderId = order.OrderId,
                            ProductId = productIds[i],
                            Quantity = quantities[i]
                        };
                        _context.OrderProducts.Add(orderProduct);
                    }

                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Wyjątek w trakcie zapisywania zamówienia: {ex.Message}");
                    ModelState.AddModelError("", "Unexpected error while saving the order.");
                }
            }
            ViewData["Products"] = new SelectList(_context.Products, "ProductId", "Name");
            return View(order);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderProducts)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            ViewData["Products"] = _context.Products.Select(p => new SelectListItem
            {
                Value = p.ProductId.ToString(),
                Text = p.Name
            }).ToList();

            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId,CustomerName,OrderDate")] Order order, int[] productIds, int[] quantities)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (productIds == null || quantities == null || productIds.Length != quantities.Length)
            {
                ModelState.AddModelError("", "Invalid products or quantities.");
            }

            if (productIds != null &&
                productIds.GroupBy(p => p).Any(g => g.Count() > 1))
            {
                ModelState.AddModelError("", "Duplicate products are not allowed.");
            }

            if (productIds != null &&
                productIds.Any(p => p == 0))
            {
                ModelState.AddModelError("", "All product selections must be valid.");
            }

            if (quantities != null && quantities.Any(q => q <= 0))
            {
                ModelState.AddModelError("", "All quantities must be greater than zero.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update the order
                    _context.Update(order);

                    // Remove existing OrderProducts
                    var existingOrderProducts = _context.OrderProducts.Where(op => op.OrderId == id);
                    _context.OrderProducts.RemoveRange(existingOrderProducts);

                    // Add updated OrderProducts
                    for (int i = 0; i < productIds.Length; i++)
                    {
                        var orderProduct = new OrderProduct
                        {
                            OrderId = id,
                            ProductId = productIds[i],
                            Quantity = quantities[i]
                        };
                        _context.OrderProducts.Add(orderProduct);
                    }

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData["Products"] = new SelectList(_context.Products, "ProductId", "Name");
            return View(order);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .Include(o => o.OrderProducts)
                    .ThenInclude(op => op.Product)
                .FirstOrDefaultAsync(m => m.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders
                .Include(o => o.OrderProducts)
                .FirstOrDefaultAsync(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            _context.OrderProducts.RemoveRange(order.OrderProducts);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
