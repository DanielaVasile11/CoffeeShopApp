using Microsoft.AspNetCore.Mvc;
using CoffeeShopAPI.Data;
using CoffeeShopAPI.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http; // Necesar pentru Sesiuni

namespace CoffeeShopAPI.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        // 1. COȘUL CURENT (Static)
        public static List<Product> ShoppingCart = new List<Product>();

        // 2. ISTORICUL COMENZILOR (Static - Nou)
        public static List<Order> OrderHistory = new List<Order>();

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Cart/Index
        public IActionResult Index()
        {
            return View(ShoppingCart);
        }

        // Adaugă în coș
        public IActionResult AddToCart(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null)
            {
                ShoppingCart.Add(product);
            }
            return RedirectToAction("Index");
        }

        // Șterge din coș
        public IActionResult RemoveFromCart(int id)
        {
            var product = ShoppingCart.FirstOrDefault(p => p.Id == id);
            if (product != null)
            {
                ShoppingCart.Remove(product);
            }
            return RedirectToAction("Index");
        }

        // ==========================================
        // CHECKOUT & PROCESARE COMANDĂ
        // ==========================================

        public IActionResult Checkout()
        {
            if (ShoppingCart.Count == 0) return RedirectToAction("Index", "Products");
            ViewBag.Total = ShoppingCart.Sum(p => p.Price);
            return View();
        }

        [HttpPost]
        public IActionResult Checkout(string fullName, string address, string phone)
        {
            // 1. Preluăm utilizatorul logat (sau "Anonim" dacă nu e logat)
            string currentUser = HttpContext.Session.GetString("Username") ?? "Anonim";

            // 2. Creăm obiectul Comandă
            var newOrder = new Order
            {
                OrderId = "#CMD-" + new Random().Next(1000, 9999), // Generăm un ID random
                Username = currentUser,
                CustomerName = fullName,
                Address = address,
                OrderDate = DateTime.Now,
                TotalAmount = ShoppingCart.Sum(p => p.Price),
                // IMPORTANT: Facem o copie a listei (clone), altfel se șterge când golim coșul
                OrderedItems = new List<Product>(ShoppingCart)
            };

            // 3. Salvăm comanda în Istoric
            OrderHistory.Add(newOrder);

            // 4. Golim coșul
            ShoppingCart.Clear();

            return RedirectToAction("OrderConfirmation");
        }

        public IActionResult OrderConfirmation()
        {
            return View();
        }

        // ==========================================
        // ISTORIC COMENZI (NOU)
        // ==========================================
        public IActionResult History()
        {
            string currentUser = HttpContext.Session.GetString("Username");

            // Dacă e Admin, vede TOATE comenzile
            if (HttpContext.Session.GetString("UserRole") == "Admin")
            {
                // Le returnăm invers cronologic (cele mai noi primele)
                var allOrders = OrderHistory.OrderByDescending(o => o.OrderDate).ToList();
                ViewData["Title"] = "Toate Comenzile (Admin)";
                return View(allOrders);
            }

            // Dacă e User Simplu, vede DOAR comenzile lui
            if (!string.IsNullOrEmpty(currentUser))
            {
                var myOrders = OrderHistory
                    .Where(o => o.Username == currentUser)
                    .OrderByDescending(o => o.OrderDate)
                    .ToList();
                ViewData["Title"] = "Istoricul Meu";
                return View(myOrders);
            }

            // Dacă nu e logat deloc
            return RedirectToAction("Login", "Account");
        }
    }
}