using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RunaTalento.Data;
using RunaTalento.Models;

namespace RunaTalento.Controllers
{
    /// <summary>
    /// Controlador para gestionar incentivos automáticos
    /// </summary>
    [Authorize(Roles = "Admin")]
    public class IncentivoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IncentivoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Incentivo/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Incentivo/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Descripcion,PuntosRequeridos,IconoUrl,Activo")] Incentivo incentivo)
        {
            if (ModelState.IsValid)
            {
                incentivo.FechaCreacion = DateTime.Now;
                _context.Add(incentivo);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Incentivo creado exitosamente";
                return RedirectToAction("Index", "Incentivos");
            }
            return View(incentivo);
        }

        // GET: Incentivo/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incentivo = await _context.Incentivo.FindAsync(id);
            if (incentivo == null)
            {
                return NotFound();
            }
            return View(incentivo);
        }

        // POST: Incentivo/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdIncentivo,Nombre,Descripcion,PuntosRequeridos,IconoUrl,Activo,FechaCreacion")] Incentivo incentivo)
        {
            if (id != incentivo.IdIncentivo)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(incentivo);
                    await _context.SaveChangesAsync();
                    TempData["Success"] = "Incentivo actualizado exitosamente";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IncentivoExists(incentivo.IdIncentivo))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index", "Incentivos");
            }
            return View(incentivo);
        }

        // GET: Incentivo/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var incentivo = await _context.Incentivo
                .Include(i => i.IncentivoEstudiantes)
                .FirstOrDefaultAsync(m => m.IdIncentivo == id);

            if (incentivo == null)
            {
                return NotFound();
            }

            return View(incentivo);
        }

        // POST: Incentivo/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var incentivo = await _context.Incentivo.FindAsync(id);
            if (incentivo != null)
            {
                _context.Incentivo.Remove(incentivo);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Incentivo eliminado exitosamente";
            }

            return RedirectToAction("Index", "Incentivos");
        }

        // POST: Incentivo/ToggleActivo/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleActivo(int id)
        {
            var incentivo = await _context.Incentivo.FindAsync(id);
            if (incentivo == null)
            {
                return NotFound();
            }

            incentivo.Activo = !incentivo.Activo;
            await _context.SaveChangesAsync();

            TempData["Success"] = incentivo.Activo 
                ? $"Incentivo '{incentivo.Nombre}' activado. Se otorgará automáticamente" 
                : $"Incentivo '{incentivo.Nombre}' desactivado";

            return RedirectToAction("Index", "Incentivos");
        }

        private bool IncentivoExists(int id)
        {
            return _context.Incentivo.Any(e => e.IdIncentivo == id);
        }
    }
}
