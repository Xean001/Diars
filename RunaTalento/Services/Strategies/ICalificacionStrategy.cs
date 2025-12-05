namespace RunaTalento.Services.Strategies
{
    /// <summary>
    /// Patrón STRATEGY (GoF) - Interfaz para diferentes estrategias de calificación
    /// Permite cambiar el algoritmo de calificación sin modificar el código cliente
    /// </summary>
    public interface ICalificacionStrategy
    {
        int CalcularPuntaje(int puntajeMaximo, int puntajeObtenido, bool entregadoATiempo);

        string NombreEstrategia { get; }
    }
}
