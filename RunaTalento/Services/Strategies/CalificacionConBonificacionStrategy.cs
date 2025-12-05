namespace RunaTalento.Services.Strategies
{
    /// <summary>
    /// Patrón STRATEGY (GoF) - Estrategia de calificación con bonificación
    /// Otorga 10% extra si se entrega antes de la fecha límite
    /// </summary>
    public class CalificacionConBonificacionStrategy : ICalificacionStrategy
    {
        public string NombreEstrategia => "Calificación con Bonificación por Puntualidad";

        public int CalcularPuntaje(int puntajeMaximo, int puntajeObtenido, bool entregadoATiempo)
        {
            if (puntajeObtenido < 0) return 0;
            if (puntajeObtenido > puntajeMaximo) puntajeObtenido = puntajeMaximo;

            if (entregadoATiempo)
            {
                int bonificacion = (int)(puntajeObtenido * 0.10);
                puntajeObtenido += bonificacion;
            }

            return Math.Min(puntajeObtenido, puntajeMaximo);
        }
    }
}
