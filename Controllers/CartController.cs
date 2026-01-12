using Microsoft.AspNetCore.Mvc;
using CoffeeShopAPI.Data;
using CoffeeShopAPI.Models;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Stripe; // Pachetul Stripe
using Stripe.Checkout; // Funcționalitatea de Checkout

namespace CoffeeShopAPI.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;

        // COȘUL și ISTORICUL (Statice)
        public static List<CoffeeShopAPI.Models.Product> ShoppingCart = new List<CoffeeShopAPI.Models.Product>();
        public static List<Order> OrderHistory = new List<Order>();

       
        // Aceasta este o cheie de TEST publică de la Stripe
        private const string StripeSecretKey = "CHEIE_TEST";

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View(ShoppingCart);
        }

        public IActionResult AddToCart(int id)
        {
            var product = _context.Products.Find(id);
            if (product != null) ShoppingCart.Add(product);
            return RedirectToAction("Index");
        }

        public IActionResult RemoveFromCart(int id)
        {
            var product = ShoppingCart.FirstOrDefault(p => p.Id == id);
            if (product != null) ShoppingCart.Remove(product);
            return RedirectToAction("Index");
        }

        // ==========================================
        // STRIPE CHECKOUT FLOW
        // ==========================================

        // Pasul 1: Inițiem plata
        public IActionResult Checkout()
        {
            if (ShoppingCart.Count == 0) return RedirectToAction("Index");

            // Configurare Stripe
            
            StripeConfiguration.ApiKey = "CHEIE_STRIPE"; 

            var domain = "https://localhost:7255"; 

            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + "/Cart/OrderConfirmation", // Unde se întoarce după plată reușită
                CancelUrl = domain + "/Cart/Index",              // Unde se întoarce dacă dă Cancel
            };

            foreach (var item in ShoppingCart)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Price * 100), // Stripe folosește bani (cents), deci înmulțim cu 100
                        Currency = "ron",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Name,
                            Description = item.Description ?? "Produs CoffeeShop"
                        },
                    },
                    Quantity = 1,
                });
            }

            var service = new SessionService();
            Session session = service.Create(options);

            // Redirecționăm utilizatorul către pagina securizată Stripe
            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);
        }

        // Plata Reușită (Success URL)
        public IActionResult OrderConfirmation()
        {
            // Salvăm comanda în istoric înainte de a goli coșul
            string currentUser = HttpContext.Session.GetString("Username") ?? "Client Stripe";

            var newOrder = new Order
            {
                OrderId = "#STRIPE-" + new Random().Next(10000, 99999),
                Username = currentUser,
                CustomerName = currentUser, // Stripe nu ne returnează numele direct aici fără webhook-uri complexe
                Address = "Procesat online prin Stripe",
                OrderDate = DateTime.Now,
                TotalAmount = ShoppingCart.Sum(p => p.Price),
                OrderedItems = new List<CoffeeShopAPI.Models.Product>(ShoppingCart)
            };

            OrderHistory.Add(newOrder);

            // Golim coșul
            ShoppingCart.Clear();

            return View();
        }

        // ISTORIC (Rămâne neschimbat)
        public IActionResult History()
        {
            string currentUser = HttpContext.Session.GetString("Username");
            if (HttpContext.Session.GetString("UserRole") == "Admin")
            {
                var allOrders = OrderHistory.OrderByDescending(o => o.OrderDate).ToList();
                ViewData["Title"] = "Toate Comenzile (Admin)";
                return View(allOrders);
            }
            if (!string.IsNullOrEmpty(currentUser))
            {
                var myOrders = OrderHistory.Where(o => o.Username == currentUser).OrderByDescending(o => o.OrderDate).ToList();
                ViewData["Title"] = "Istoricul Meu";
                return View(myOrders);
            }
            return RedirectToAction("Login", "Account");
        }
    }
}