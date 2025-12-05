namespace RunaTalento.Services.Strategies
{
    /// <summary>
    /// Patrón STRATEGY (GoF) - Estrategia de calificación con penalización
    /// Reduce 20% si se entrega después de la fecha límite
    /// </summary>
    public class CalificacionConPenalizacionStrategy : ICalificacionStrategy
    {
        public string NombreEstrategia => "Calificación con Penalización por Retraso";

        public int CalcularPuntaje(int puntajeMaximo, int puntajeObtenido, bool entregadoATiempo)
        {
            if (puntajeObtenido < 0) return 0;
            if (puntajeObtenido > puntajeMaximo) puntajeObtenido = puntajeMaximo;

            if (!entregadoATiempo)
            {
                int penalizacion = (int)(puntajeObtenido * 0.20);
                puntajeObtenido -= penalizacion;
            }

            return Math.Max(puntajeObtenido, 0);
        }
    }
}
