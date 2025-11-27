using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoffeeShopAPI.Data;
using CoffeeShopAPI.Models;

// NOU: Acum moștenește din clasa Controller
public class ProductsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ProductsController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: /Products/Index (Funcționalitatea "Vizualizare meniu" din Raport)
    // NOU: Returnează IActionResult (o View, un Redirect, etc.)
    public async Task<IActionResult> Index()
    {
        // Extrage lista de produse din baza de date
        var products = await _context.Products.ToListAsync();

        // Trimite lista de produse ca model către View-ul "Index.cshtml"
        return View(products);
    }

    // NOTĂ: Dacă ai nevoie de un endpoint Admin pentru adăugare/editare produse
    // Ar trebui să adaugi metode precum public IActionResult Create() și [HttpPost] public IActionResult Create(Product product)
}