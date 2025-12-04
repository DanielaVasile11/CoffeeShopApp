using Microsoft.AspNetCore.Mvc;
using CoffeeShopAPI.Data; // Acces la DB
using CoffeeShopAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace CoffeeShopAPI.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        // LISTA STATICĂ - Simulează coșul de cumpărături pentru sesiunea curentă
        public static List<Product> ShoppingCart = new List<Product>();

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Cart/Index (Afișează coșul)
        public IActionResult Index()
        {
            // Trimitem lista statică către View
            return View(ShoppingCart);
        }

        // GET: /Cart/AddToCart/5 (Adaugă produs)
        public IActionResult AddToCart(int id)
        {
            // Căutăm produsul în baza de date reală
            var product = _context.Products.Find(id);

            if (product != null)
            {
                // Îl adăugăm în lista noastră temporară
                ShoppingCart.Add(product);
            }

            // Redirecționăm utilizatorul la pagina Coșului pentru a vedea rezultatul
            return RedirectToAction("Index");
        }

        // GET: /Cart/RemoveFromCart/5 (Șterge produs - Opțional pentru demo)
        public IActionResult RemoveFromCart(int id)
        {
            var product = ShoppingCart.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                ShoppingCart.Remove(product);
            }
            return RedirectToAction("Index");
        }
    }
}