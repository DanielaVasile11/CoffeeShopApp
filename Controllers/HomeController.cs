using Microsoft.AspNetCore.Mvc;

namespace CoffeeShopAPI.Controllers
{
    public class HomeController : Controller
    {
        // Această metodă leagă ruta "/" de fișierul Views/Home/Index.cshtml
        public IActionResult Index()
        {
            return View();
        }
    }
}