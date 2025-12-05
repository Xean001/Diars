using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunaTalento.Data;
using RunaTalento.Models;

namespace RunaTalento.Controllers
{
    /// <summary>
    /// Controlador para gestionar incentivos (Medallas y Niveles)
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class IncentivosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IncentivosController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Incentivos
        public async Task<IActionResult> Index()
        {
            await SetHeaderDataAsync();

            // Obtener medallas, niveles e incentivos automáticos
            var medallas = await _context.Medalla.ToListAsync();
            var niveles = await _context.Nivel.OrderBy(n => n.PuntajeMinimo).ToListAsync();
            var incentivos = await _context.Incentivo
                .Include(i => i.IncentivoEstudiantes)
                .OrderBy(i => i.PuntosRequeridos)
                .ToListAsync();

            ViewBag.Medallas = medallas;
            ViewBag.Niveles = niveles;
            ViewBag.Incentivos = incentivos; // ? NUEVO

            return View();
        }

        // Método auxiliar para establecer datos del header
        private async Task SetHeaderDataAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (userId != null)
            {
                var admin = await _context.Users.FindAsync(userId);
                if (admin != null)
                {
                    ViewBag.NombreCompleto = $"{admin.Nombres} {admin.Apellidos}";
                    var nombres = admin.Nombres?.Split(' ');
                    var apellidos = admin.Apellidos?.Split(' ');
                    ViewBag.Iniciales = $"{nombres?[0]?.Substring(0, 1)}{apellidos?[0]?.Substring(0, 1)}";
                }
            }
        }
    }
}
