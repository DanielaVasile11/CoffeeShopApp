using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoffeeShopAPI.Data;
using CoffeeShopAPI.Models;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace CoffeeShopAPI.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =================================================================
        // GET: /Products/Index (Meniu)
        // =================================================================
        public async Task<IActionResult> Index(string SearchString, string category)
        {
            var products = from p in _context.Products select p;

            if (!string.IsNullOrEmpty(category))
            {
                products = products.Where(p => p.Category == category);
                ViewData["Title"] = "Meniu - " + category;
            }

            if (!string.IsNullOrEmpty(SearchString))
            {
                products = products.Where(p => p.Name.Contains(SearchString) || p.Description.Contains(SearchString));
                ViewData["Title"] = "Rezultate căutare";
            }

            if (string.IsNullOrEmpty(category) && string.IsNullOrEmpty(SearchString))
            {
                ViewData["Title"] = "Meniul Cafenelei (Toate Produsele)";
            }

            return View(await products.ToListAsync());
        }

        // =================================================================
        // ADMIN: CREATE
        // =================================================================
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin") return RedirectToAction("Login", "Account");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Price,Description,Category")] Product product)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin") return RedirectToAction("Login", "Account");

            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // =================================================================
        // ADMIN: EDIT (MODIFICARE PRODUS) - NOU
        // =================================================================

        // GET: Deschide formularul de editare
        public async Task<IActionResult> Edit(int? id)
        {
            // Verificare Admin
            if (HttpContext.Session.GetString("UserRole") != "Admin") return RedirectToAction("Login", "Account");

            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            return View(product);
        }

        // POST: Salvează modificările
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Price,Description,Category")] Product product)
        {
            // Verificare Admin
            if (HttpContext.Session.GetString("UserRole") != "Admin") return RedirectToAction("Login", "Account");

            if (id != product.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Products.Any(e => e.Id == product.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // =================================================================
        // ADMIN: DELETE
        // =================================================================
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin") return RedirectToAction("Login", "Account");

            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // =================================================================
        // DETAILS
        // =================================================================
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products.FirstOrDefaultAsync(m => m.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }
    }
}