using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunaTalento.Data;
using RunaTalento.Models;

namespace RunaTalento.Controllers
{
    /// <summary>
    /// Controlador para gestionar medallas e insignias
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class MedallaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedallaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Medalla
        public async Task<IActionResult> Index()
        {
            var medallas = await _context.Medalla.ToListAsync();
            return View(medallas);
        }

        // GET: Medalla/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medalla = await _context.Medalla
                .Include(m => m.MedallasEstudiantes)
                    .ThenInclude(me => me.Estudiante)
                .Include(m => m.MedallasEstudiantes)
                    .ThenInclude(me => me.Docente)
                .FirstOrDefaultAsync(m => m.IdMedalla == id);
            
            if (medalla == null)
            {
                return NotFound();
            }

            return View(medalla);
        }

        // GET: Medalla/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Medalla/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdMedalla,Nombre,Descripcion,ImagenUrl")] Medalla medalla)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medalla);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Medalla creada exitosamente";
                return RedirectToAction(nameof(Index));
            }
            return View(medalla);
        }

        // GET: Medalla/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medalla = await _context.Medalla.FindAsync(id);
            if (medalla == null)
            {
                return NotFound();
            }
            return View(medalla);
        }

        // POST: Medalla/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdMedalla,Nombre,Descripcion,ImagenUrl")] Medalla medalla)
        {
            if (id != medalla.IdMedalla)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medalla);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Medalla actualizada exitosamente";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedallaExists(medalla.IdMedalla))
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
            return View(medalla);
        }

        // GET: Medalla/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medalla = await _context.Medalla
                .Include(m => m.MedallasEstudiantes)
                .FirstOrDefaultAsync(m => m.IdMedalla == id);
            
            if (medalla == null)
            {
                return NotFound();
            }

            return View(medalla);
        }

        // POST: Medalla/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medalla = await _context.Medalla.FindAsync(id);
            if (medalla != null)
            {
                _context.Medalla.Remove(medalla);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Medalla eliminada exitosamente";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MedallaExists(int id)
        {
            return _context.Medalla.Any(e => e.IdMedalla == id);
        }
    }
}
