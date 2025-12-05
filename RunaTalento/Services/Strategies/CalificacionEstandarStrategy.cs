namespace RunaTalento.Services.Strategies
{
    /// <summary>
    /// Patrón STRATEGY (GoF) - Estrategia de calificación estándar
    /// Aplica el puntaje tal cual fue asignado por el docente
    /// </summary>
    public class CalificacionEstandarStrategy : ICalificacionStrategy
    {
        public string NombreEstrategia => "Calificación Estándar";

        public int CalcularPuntaje(int puntajeMaximo, int puntajeObtenido, bool entregadoATiempo)
        {
            if (puntajeObtenido < 0) return 0;
            if (puntajeObtenido > puntajeMaximo) return puntajeMaximo;
            
            return puntajeObtenido;
        }
    }
}
