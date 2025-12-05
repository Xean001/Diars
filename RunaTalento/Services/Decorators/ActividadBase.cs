namespace RunaTalento.Services.Decorators
{
    using RunaTalento.Models;

    /// <summary>
    /// Patrón DECORATOR (GoF) - Componente concreto base
    /// Representa una actividad sin decoraciones adicionales
    /// </summary>
    public class ActividadBase : IActividadComponent
    {
        private readonly Actividad _actividad;

        public ActividadBase(Actividad actividad)
        {
            _actividad = actividad;
        }

        public int IdActividad => _actividad.IdActividad;
        public string Titulo => _actividad.Titulo;
        public string Descripcion => _actividad.Descripcion ?? "";
        public int Puntaje => _actividad.Puntaje;

        public virtual string ObtenerDescripcionCompleta()
        {
            return $"{Titulo}: {Descripcion}";
        }

        public virtual int CalcularPuntajeConModificadores()
        {
            return Puntaje;
        }
    }
}
