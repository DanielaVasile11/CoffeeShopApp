using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; // Necesar pentru Session

namespace CoffeeShopAPI.Controllers
{
    public class AccountController : Controller
    {
        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            // Verificare Simplă (Hardcodată pentru demo)

            // 1. Cazul ADMIN
            if (username == "admin" && password == "1234")
            {
                // Salvăm în sesiune că e logat și e Admin
                HttpContext.Session.SetString("UserRole", "Admin");
                HttpContext.Session.SetString("Username", "Administrator");
                return RedirectToAction("Index", "Home");
            }

            // 2. Cazul USER (CLIENT)
            if (username == "client" && password == "1234")
            {
                // Salvăm în sesiune că e logat și e User simplu
                HttpContext.Session.SetString("UserRole", "User");
                HttpContext.Session.SetString("Username", "Client Fidel");
                return RedirectToAction("Index", "Home");
            }

            // 3. Dacă datele sunt greșite
            ViewBag.Error = "Nume sau parolă incorectă!";
            return View();
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            // Ștergem sesiunea (delogare)
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}