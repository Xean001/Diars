using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunaTalento.Data;
using RunaTalento.Models;

namespace RunaTalento.Controllers
{
    /// <summary>
    /// Controlador para que los estudiantes vean y realicen actividades
    /// </summary>
    [Authorize(Roles = "Estudiante")]
    public class ActividadesEstudianteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ActividadesEstudianteController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: ActividadesEstudiante
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);

            // Obtener SOLO actividades vigentes (no vencidas) con información de si el estudiante ya las completó
            var actividades = await _context.Actividad
                .Include(a => a.Curso)
                .ThenInclude(c => c.Docente)
                .Where(a => !a.FechaLimite.HasValue || a.FechaLimite.Value >= DateTime.Now) // ? FILTRAR ACTIVIDADES VIGENTES
                .Select(a => new
                {
                    Actividad = a,
                    ActividadEstudiante = _context.ActividadEstudiante
                        .FirstOrDefault(ae => ae.IdActividad == a.IdActividad && ae.IdEstudiante == userId)
                })
                .ToListAsync();

            // Datos para el header del layout
            await SetHeaderDataAsync(userId);

            ViewBag.UserId = userId;
            return View(actividades);
        }

        // GET: ActividadesEstudiante/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            var actividad = await _context.Actividad
                .Include(a => a.Curso)
                .ThenInclude(c => c.Docente)
                .FirstOrDefaultAsync(m => m.IdActividad == id);

            if (actividad == null)
            {
                return NotFound();
            }

            // Verificar si el estudiante ya completó esta actividad
            var actividadEstudiante = await _context.ActividadEstudiante
                .FirstOrDefaultAsync(ae => ae.IdActividad == id && ae.IdEstudiante == userId);

            ViewBag.ActividadEstudiante = actividadEstudiante;
            ViewBag.YaCompletada = actividadEstudiante != null;

            // Datos para el header
            await SetHeaderDataAsync(userId);

            return View(actividad);
        }

        // GET: ActividadesEstudiante/Realizar/5
        public async Task<IActionResult> Realizar(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userId = _userManager.GetUserId(User);

            // Verificar si ya completó la actividad
            var yaCompletada = await _context.ActividadEstudiante
                .AnyAsync(ae => ae.IdActividad == id && ae.IdEstudiante == userId);

            if (yaCompletada)
            {
                TempData["Warning"] = "Ya has completado esta actividad";
                return RedirectToAction(nameof(Index));
            }

            var actividad = await _context.Actividad
                .Include(a => a.Curso)
                .ThenInclude(c => c.Docente)
                .FirstOrDefaultAsync(m => m.IdActividad == id);

            if (actividad == null)
            {
                return NotFound();
            }

            // ? VERIFICAR SI LA ACTIVIDAD ESTÁ VENCIDA
            if (actividad.FechaLimite.HasValue && DateTime.Now > actividad.FechaLimite.Value)
            {
                TempData["Error"] = "Esta actividad ya venció. No puedes entregarla.";
                return RedirectToAction(nameof(Index));
            }

            // Datos para el header
            await SetHeaderDataAsync(userId);

            return View(actividad);
        }

        // POST: ActividadesEstudiante/Realizar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Realizar(int id, IFormFile? archivoEntrega)
        {
            var userId = _userManager.GetUserId(User);

            // Verificar que la actividad existe
            var actividad = await _context.Actividad.FindAsync(id);
            if (actividad == null)
            {
                return NotFound();
            }

            // ? VERIFICAR SI LA ACTIVIDAD ESTÁ VENCIDA
            if (actividad.FechaLimite.HasValue && DateTime.Now > actividad.FechaLimite.Value)
            {
                TempData["Error"] = "Esta actividad ya venció. No puedes entregarla.";
                return RedirectToAction(nameof(Index));
            }

            // Verificar si ya completó la actividad
            var yaCompletada = await _context.ActividadEstudiante
                .AnyAsync(ae => ae.IdActividad == id && ae.IdEstudiante == userId);

            if (yaCompletada)
            {
                TempData["Warning"] = "Ya has completado esta actividad";
                return RedirectToAction(nameof(Index));
            }

            // Crear la entrega
            var actividadEstudiante = new ActividadEstudiante
            {
                IdActividad = id,
                IdEstudiante = userId,
                FechaEntrega = DateTime.Now,
                PuntajeObtenido = null // El docente lo calificará después
            };

            // ? Implementar subida de archivo
            if (archivoEntrega != null && archivoEntrega.Length > 0)
            {
                try
                {
                    // Validar tamaño (máximo 10MB)
                    if (archivoEntrega.Length > 10 * 1024 * 1024)
                    {
                        TempData["Error"] = "El archivo no debe superar los 10MB";
                        return RedirectToAction("Realizar", new { id });
                    }

                    // Crear carpeta si no existe
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "actividades");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    // Generar nombre único para el archivo
                    var extension = Path.GetExtension(archivoEntrega.FileName);
                    var uniqueFileName = $"{userId}_{id}_{DateTime.Now:yyyyMMddHHmmss}{extension}";
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Guardar archivo
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await archivoEntrega.CopyToAsync(fileStream);
                    }

                    // Guardar URL relativa en la base de datos
                    actividadEstudiante.UrlDocumento = $"/uploads/actividades/{uniqueFileName}";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al subir el archivo: {ex.Message}";
                    return RedirectToAction("Realizar", new { id });
                }
            }

            _context.ActividadEstudiante.Add(actividadEstudiante);
            await _context.SaveChangesAsync();

            TempData["Success"] = archivoEntrega != null 
                ? "Actividad completada exitosamente con documento adjunto. El docente la calificará pronto."
                : "Actividad completada exitosamente. El docente la calificará pronto.";
            
            return RedirectToAction(nameof(Index));
        }

        // Método auxiliar para establecer datos del header
        private async Task SetHeaderDataAsync(string userId)
        {
            var estudiante = await _context.Users
                .Include(u => u.Nivel)
                .FirstOrDefaultAsync(u => u.Id == userId);
                
            if (estudiante != null)
            {
                ViewBag.PuntajeTotal = estudiante.PuntajeTotal;
                // ? MOSTRAR "Nivel X" en lugar del nombre del nivel
                ViewBag.NivelTexto = estudiante.IdNivel.HasValue 
                    ? $"Nivel {estudiante.IdNivel}" 
                    : "Sin nivel";
                ViewBag.NivelId = estudiante.IdNivel ?? 0;
                ViewBag.NombreCompleto = $"{estudiante.Nombres} {estudiante.Apellidos}";
                var nombres = estudiante.Nombres?.Split(' ');
                var apellidos = estudiante.Apellidos?.Split(' ');
                ViewBag.Iniciales = $"{nombres?[0]?.Substring(0, 1)}{apellidos?[0]?.Substring(0, 1)}";
            }
        }
    }
}
