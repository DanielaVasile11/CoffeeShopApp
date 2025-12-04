using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoffeeShopAPI.Data;
using CoffeeShopAPI.Models;
using System.Linq; // Necesar pentru funcțiile de filtrare Where() și Contains()

// Controller-ul moștenește din Controller (pentru MVC)
public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // =================================================================
    // GET: /Products/Index (Afișează Meniul cu Filtre/Căutare)
    // =================================================================
    public async Task<IActionResult> Index(string SearchString, string category)
    {
        // Începem interogarea cu toate produsele
        var products = from p in _context.Products
                       select p;

        // 1. FILTRARE după Categorie (din butoanele Navbar)
        if (!string.IsNullOrEmpty(category))
        {
            products = products.Where(p => p.Category == category);
            ViewData["Title"] = "Meniu - " + category;
        }

        // 2. CĂUTARE (din formularul de căutare din Navbar)
        if (!string.IsNullOrEmpty(SearchString))
        {
            // Filtrează după Nume SAU Descriere
            products = products.Where(p => p.Name.Contains(SearchString) || p.Description.Contains(SearchString));
            ViewData["Title"] = "Rezultate căutare";
        }

        // Setează titlul standard dacă nu există filtre
        if (string.IsNullOrEmpty(category) && string.IsNullOrEmpty(SearchString))
        {
            ViewData["Title"] = "Meniul Cafenelei (Toate Produsele)";
        }

        // Trimite lista filtrată/căutată către View
        return View(await products.ToListAsync());
    }

    // =================================================================
    // ADMIN FUNCTIONALITY: ADD PRODUCT
    // =================================================================

    // GET: /Products/Create (Afișează formularul)
    public IActionResult Create()
    {
        return View();
    }

    // POST: /Products/Create (Procesează trimiterea formularului)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Name,Price,Description,Category")] Product product)
    {
        // Dacă validarea pe partea de server trece
        if (ModelState.IsValid)
        {
            _context.Add(product);
            await _context.SaveChangesAsync();

            // Redirecționează la meniu după adăugare
            return RedirectToAction(nameof(Index));
        }

        // Reafișează formularul dacă sunt erori de validare
        return View(product);
    }

    // În ProductsController.cs

    // GET: /Products/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var product = await _context.Products
            .FirstOrDefaultAsync(m => m.Id == id);

        if (product == null)
        {
            return NotFound();
        }

        return View(product);
    }
}