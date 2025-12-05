using RunaTalento.Models;
using RunaTalento.Services.Strategies;
using RunaTalento.Services.Observers;

namespace RunaTalento.BusinessLogic
{
    /// <summary>
    /// Patrón CONTROLLER (GRASP) - Lógica de negocio para calificaciones
    /// Coordina las operaciones de calificación sin acoplarse a la capa de presentación
    /// Responsabilidad: Aplicar reglas de negocio y orquestar patrones (Strategy, Observer)
    /// </summary>
    public class CalificacionBusinessLogic
    {
        private readonly ICalificacionStrategy _strategy;
        private readonly CalificacionNotifier _notifier;

        public CalificacionBusinessLogic(
            ICalificacionStrategy strategy,
            CalificacionNotifier notifier)
        {
            _strategy = strategy;
            _notifier = notifier;
        }

        /// <summary>
        /// Procesa la calificación de una actividad aplicando estrategias y notificando observers
        /// </summary>
        public ResultadoCalificacion ProcesarCalificacion(
            ActividadEstudiante actividadEstudiante,
            int puntajeAsignado,
            DateTime? fechaLimite)
        {
            if (actividadEstudiante == null)
                throw new ArgumentNullException(nameof(actividadEstudiante));

            if (puntajeAsignado < 0)
                throw new ArgumentException("El puntaje no puede ser negativo", nameof(puntajeAsignado));

            // Determinar si la entrega fue a tiempo
            bool entregadoATiempo = !fechaLimite.HasValue || 
                                   actividadEstudiante.FechaEntrega <= fechaLimite.Value;

            // Aplicar estrategia de calificación (Patrón Strategy)
            int puntajeFinal = _strategy.CalcularPuntaje(
                actividadEstudiante.Actividad.Puntaje,
                puntajeAsignado,
                entregadoATiempo);

            // Actualizar puntaje obtenido
            actividadEstudiante.PuntajeObtenido = puntajeFinal;

            // Notificar a observers (Patrón Observer)
            _notifier.NotificarCalificacion(
                actividadEstudiante.IdEstudiante,
                actividadEstudiante.IdActividad,
                puntajeFinal);

            // Retornar resultado encapsulado
            return new ResultadoCalificacion
            {
                PuntajeOriginal = puntajeAsignado,
                PuntajeFinal = puntajeFinal,
                EstrategiaAplicada = _strategy.NombreEstrategia,
                EntregadoATiempo = entregadoATiempo,
                Mensaje = GenerarMensajeResultado(puntajeAsignado, puntajeFinal, entregadoATiempo)
            };
        }

        /// <summary>
        /// Genera mensaje descriptivo del resultado de la calificación
        /// </summary>
        private string GenerarMensajeResultado(int puntajeAsignado, int puntajeFinal, bool entregadoATiempo)
        {
            if (puntajeFinal > puntajeAsignado)
            {
                int diferencia = puntajeFinal - puntajeAsignado;
                return $"Puntaje bonificado: {puntajeAsignado} + {diferencia} = {puntajeFinal} puntos (Entrega puntual)";
            }
            else if (puntajeFinal < puntajeAsignado)
            {
                int diferencia = puntajeAsignado - puntajeFinal;
                return $"Puntaje con penalización: {puntajeAsignado} - {diferencia} = {puntajeFinal} puntos (Entrega tardía)";
            }
            else
            {
                return $"Puntaje estándar: {puntajeFinal} puntos";
            }
        }
    }

    /// <summary>
    /// DTO que encapsula el resultado de una calificación
    /// Patrón Information Expert (GRASP) - Encapsula datos relacionados
    /// </summary>
    public class ResultadoCalificacion
    {
        public int PuntajeOriginal { get; set; }
        public int PuntajeFinal { get; set; }
        public string EstrategiaAplicada { get; set; }
        public bool EntregadoATiempo { get; set; }
        public string Mensaje { get; set; }
    }
}
