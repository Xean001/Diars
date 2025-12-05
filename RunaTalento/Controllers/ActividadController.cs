using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RunaTalento.Data;
using RunaTalento.Models;

namespace RunaTalento.Controllers
{
    /// <summary>
    /// Controlador para gestionar actividades
    /// </summary>
    [Authorize(Roles = "Docente")]
    public class ActividadController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ActividadController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Actividad
        public async Task<IActionResult> Index()
        {
            var actividades = await _context.Actividad
                .Include(a => a.Curso)
                .ThenInclude(c => c.Docente)
                .ToListAsync();
            return View(actividades);
        }

        // GET: Actividad/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actividad = await _context.Actividad
                .Include(a => a.Curso)
                .ThenInclude(c => c.Docente)
                .Include(a => a.ActividadesEstudiantes)
                .ThenInclude(ae => ae.Estudiante)
                .FirstOrDefaultAsync(m => m.IdActividad == id);

            if (actividad == null)
            {
                return NotFound();
            }

            return View(actividad);
        }

        // GET: Actividad/Create
        public IActionResult Create()
        {
            ViewData["IdCurso"] = new SelectList(_context.Curso, "IdCurso", "Nombre");
            return View();
        }

        // POST: Actividad/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdActividad,Titulo,Descripcion,Puntaje,IdCurso,FechaLimite")] Actividad actividad)
        {
            if (ModelState.IsValid)
            {
                actividad.FechaCreacion = DateTime.Now;
                _context.Add(actividad);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Actividad creada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCurso"] = new SelectList(_context.Curso, "IdCurso", "Nombre", actividad.IdCurso);
            return View(actividad);
        }

        // GET: Actividad/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actividad = await _context.Actividad.FindAsync(id);
            if (actividad == null)
            {
                return NotFound();
            }
            ViewData["IdCurso"] = new SelectList(_context.Curso, "IdCurso", "Nombre", actividad.IdCurso);
            return View(actividad);
        }

        // POST: Actividad/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdActividad,Titulo,Descripcion,Puntaje,FechaCreacion,IdCurso,FechaLimite")] Actividad actividad)
        {
            if (id != actividad.IdActividad)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(actividad);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Actividad actualizada exitosamente";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActividadExists(actividad.IdActividad))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdCurso"] = new SelectList(_context.Curso, "IdCurso", "Nombre", actividad.IdCurso);
            return View(actividad);
        }

        // GET: Actividad/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var actividad = await _context.Actividad
                .Include(a => a.Curso)
                .FirstOrDefaultAsync(m => m.IdActividad == id);

            if (actividad == null)
            {
                return NotFound();
            }

            return View(actividad);
        }

        // POST: Actividad/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var actividad = await _context.Actividad.FindAsync(id);
            if (actividad != null)
            {
                _context.Actividad.Remove(actividad);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Actividad eliminada exitosamente";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ActividadExists(int id)
        {
            return _context.Actividad.Any(e => e.IdActividad == id);
        }
    }
}