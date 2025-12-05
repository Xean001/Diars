namespace RunaTalento.Services.Observers
{
    /// <summary>
    /// Patrón OBSERVER (GoF) - Interfaz para observadores de eventos de calificación
    /// Permite notificar a múltiples componentes cuando se califica una actividad
    /// </summary>
    public interface ICalificacionObserver
    {
        void OnActividadCalificada(string estudianteId, int actividadId, int puntajeObtenido);
    }
}
