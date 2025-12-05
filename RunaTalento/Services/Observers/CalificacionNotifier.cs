namespace RunaTalento.Services.Observers
{
    /// <summary>
    /// Patrón OBSERVER (GoF) - Sujeto observable que notifica eventos de calificación
    /// Mantiene una lista de observadores y los notifica cuando ocurre un evento
    /// </summary>
    public class CalificacionNotifier
    {
        private readonly List<ICalificacionObserver> _observers = new();

        public void Attach(ICalificacionObserver observer)
        {
            if (!_observers.Contains(observer))
            {
                _observers.Add(observer);
            }
        }

        public void Detach(ICalificacionObserver observer)
        {
            _observers.Remove(observer);
        }

        public void NotificarCalificacion(string estudianteId, int actividadId, int puntajeObtenido)
        {
            foreach (var observer in _observers)
            {
                observer.OnActividadCalificada(estudianteId, actividadId, puntajeObtenido);
            }
        }
    }
}
