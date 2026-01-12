using Microsoft.AspNetCore.Mvc;
using CoffeeShopAPI.Data;
using CoffeeShopAPI.Models;
using System.Collections.Generic;
using System.Linq;

namespace CoffeeShopAPI.Controllers
{
    public class FavoritesController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Lista statică pentru Favorite (Wishlist)
        public static List<Product> Wishlist = new List<Product>();

        public FavoritesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Favorites/Index (Afișează lista)
        public IActionResult Index()
        {
            return View(Wishlist);
        }

        // GET: /Favorites/Add/5 (Adaugă la favorite)
        public IActionResult Add(int id)
        {
            // Verificăm dacă produsul există deja în listă pentru a nu-l dubla
            if (!Wishlist.Any(p => p.Id == id))
            {
                var product = _context.Products.Find(id);
                if (product != null)
                {
                    Wishlist.Add(product);
                }
            }
            // După adăugare, rămânem pe pagina de produse sau mergem la favorite?
            // Pentru feedback vizual clar, mergem la lista de favorite:
            return RedirectToAction("Index");
        }

        // GET: /Favorites/Remove/5 (Șterge din favorite)
        public IActionResult Remove(int id)
        {
            var product = Wishlist.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                Wishlist.Remove(product);
            }
            return RedirectToAction("Index");
        }

        // Metoda: Mută din Favorite în Coș
        public IActionResult MoveToCart(int id)
        {
            // 1. Găsim produsul
            var product = Wishlist.FirstOrDefault(p => p.Id == id);

            if (product != null)
            {
                // 2. Îl adăugăm în Coș (folosind lista statică din CartController)
                CartController.ShoppingCart.Add(product);

                // 3. (Opțional) Îl ștergem din Favorite după ce l-am mutat
                Wishlist.Remove(product);
            }

            return RedirectToAction("Index", "Cart");
        }
    }
}