using RunaTalento.Models;
using RunaTalento.Data;
using Microsoft.EntityFrameworkCore;

namespace RunaTalento.Services
{
    /// <summary>
    /// Patrón HIGH COHESION (GRASP) - Servicio especializado en lógica de gamificación
    /// Responsabilidad: Gestionar puntos, niveles, incentivos y estadísticas de estudiantes
    /// Mantiene alta cohesión al agrupar operaciones relacionadas con gamificación
    /// </summary>
    public class GamificacionService
    {
        private readonly ApplicationDbContext _context;

        public GamificacionService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Actualiza el puntaje total del estudiante y recalcula su nivel
        /// </summary>
        public async Task<ActualizacionGamificacion> ActualizarPuntajeYNivel(
            string estudianteId, 
            int puntosGanados)
        {
            var estudiante = await _context.Users.FindAsync(estudianteId);
            if (estudiante == null)
                throw new ArgumentException("Estudiante no encontrado", nameof(estudianteId));

            var nivelAnteriorId = estudiante.IdNivel;
            var nivelAnterior = nivelAnteriorId.HasValue
                ? await _context.Nivel.FindAsync(nivelAnteriorId.Value)
                : null;

            // Actualizar puntaje total
            estudiante.PuntajeTotal += puntosGanados;

            // Determinar nuevo nivel basado en puntaje total
            var nuevoNivel = await _context.Nivel
                .Where(n => estudiante.PuntajeTotal >= n.PuntajeMinimo
                         && estudiante.PuntajeTotal <= n.PuntajeMaximo)
                .FirstOrDefaultAsync();

            if (nuevoNivel != null)
            {
                estudiante.IdNivel = nuevoNivel.IdNivel;
            }

            await _context.SaveChangesAsync();

            bool cambioNivel = nivelAnteriorId != estudiante.IdNivel;

            return new ActualizacionGamificacion
            {
                PuntosGanados = puntosGanados,
                PuntajeTotalActual = estudiante.PuntajeTotal,
                NivelAnterior = nivelAnterior?.Nombre,
                NivelActual = nuevoNivel?.Nombre,
                CambioDeNivel = cambioNivel
            };
        }

        /// <summary>
        /// Otorga automáticamente incentivos al estudiante según su puntaje
        /// </summary>
        public async Task<List<string>> OtorgarIncentivosAutomaticos(string estudianteId)
        {
            var estudiante = await _context.Users.FindAsync(estudianteId);
            if (estudiante == null)
                return new List<string>();

            var incentivosObtenidos = new List<string>();

            // Buscar incentivos que el estudiante ha desbloqueado
            var incentivosDisponibles = await _context.Incentivo
                .Where(i => i.Activo && i.PuntosRequeridos <= estudiante.PuntajeTotal)
                .ToListAsync();

            foreach (var incentivo in incentivosDisponibles)
            {
                // Verificar si ya tiene el incentivo
                var yaLoTiene = await _context.IncentivoEstudiante
                    .AnyAsync(ie => ie.IdIncentivo == incentivo.IdIncentivo 
                                 && ie.IdEstudiante == estudianteId);

                if (!yaLoTiene)
                {
                    // Otorgar nuevo incentivo
                    var incentivoEstudiante = new IncentivoEstudiante
                    {
                        IdIncentivo = incentivo.IdIncentivo,
                        IdEstudiante = estudianteId,
                        FechaOtorgado = DateTime.Now,
                        PuntajeAlObtener = estudiante.PuntajeTotal
                    };

                    _context.IncentivoEstudiante.Add(incentivoEstudiante);
                    incentivosObtenidos.Add(incentivo.Nombre);
                }
            }

            if (incentivosObtenidos.Any())
            {
                await _context.SaveChangesAsync();
            }

            return incentivosObtenidos;
        }

        /// <summary>
        /// Obtiene las estadísticas de gamificación de un estudiante
        /// </summary>
        public async Task<EstadisticasEstudiante> ObtenerEstadisticas(string estudianteId)
        {
            var estudiante = await _context.Users
                .Include(u => u.Nivel)
                .FirstOrDefaultAsync(u => u.Id == estudianteId);

            if (estudiante == null)
                return null;

            var actividadesCompletadas = await _context.ActividadEstudiante
                .Where(ae => ae.IdEstudiante == estudianteId && ae.PuntajeObtenido.HasValue)
                .CountAsync();

            var medallasObtenidas = await _context.MedallaEstudiante
                .Where(me => me.IdEstudiante == estudianteId)
                .CountAsync();

            var incentivosObtenidos = await _context.IncentivoEstudiante
                .Where(ie => ie.IdEstudiante == estudianteId)
                .CountAsync();

            return new EstadisticasEstudiante
            {
                PuntajeTotal = estudiante.PuntajeTotal,
                NivelActual = estudiante.Nivel?.Nombre ?? "Sin nivel",
                ActividadesCompletadas = actividadesCompletadas,
                MedallasObtenidas = medallasObtenidas,
                IncentivosObtenidos = incentivosObtenidos
            };
        }
    }

    /// <summary>
    /// DTO que encapsula el resultado de una actualización de gamificación
    /// </summary>
    public class ActualizacionGamificacion
    {
        public int PuntosGanados { get; set; }
        public int PuntajeTotalActual { get; set; }
        public string NivelAnterior { get; set; }
        public string NivelActual { get; set; }
        public bool CambioDeNivel { get; set; }

        public string ObtenerMensaje()
        {
            var mensaje = $"Ganaste {PuntosGanados} puntos. Total: {PuntajeTotalActual}";
            
            if (CambioDeNivel && !string.IsNullOrEmpty(NivelAnterior) && !string.IsNullOrEmpty(NivelActual))
            {
                mensaje += $" | ¡Subiste de nivel! {NivelAnterior} ? {NivelActual}";
            }
            else if (CambioDeNivel && !string.IsNullOrEmpty(NivelActual))
            {
                mensaje += $" | ¡Alcanzaste el nivel {NivelActual}!";
            }

            return mensaje;
        }
    }

    /// <summary>
    /// DTO que encapsula las estadísticas de gamificación de un estudiante
    /// </summary>
    public class EstadisticasEstudiante
    {
        public int PuntajeTotal { get; set; }
        public string NivelActual { get; set; }
        public int ActividadesCompletadas { get; set; }
        public int MedallasObtenidas { get; set; }
        public int IncentivosObtenidos { get; set; }
    }
}
