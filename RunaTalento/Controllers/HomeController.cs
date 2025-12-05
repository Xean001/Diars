using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using RunaTalento.Models;

namespace RunaTalento.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Redirigir según el rol del usuario
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Estudiante"))
                {
                    return RedirectToAction("Index", "ActividadesEstudiante");
                }
                else if (User.IsInRole("Docente"))
                {
                    return RedirectToAction("Index", "Docente");
                }
                else if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("Index", "Usuarios");
                }
            }
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
