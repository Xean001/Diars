namespace RunaTalento.Services.Decorators
{
    /// <summary>
    /// Patrón DECORATOR (GoF) - Decorador abstracto base
    /// Define la estructura para todos los decoradores concretos
    /// </summary>
    public abstract class ActividadDecorator : IActividadComponent
    {
        protected readonly IActividadComponent _actividad;

        protected ActividadDecorator(IActividadComponent actividad)
        {
            _actividad = actividad;
        }

        public virtual int IdActividad => _actividad.IdActividad;
        public virtual string Titulo => _actividad.Titulo;
        public virtual string Descripcion => _actividad.Descripcion;
        public virtual int Puntaje => _actividad.Puntaje;

        public abstract string ObtenerDescripcionCompleta();
        public abstract int CalcularPuntajeConModificadores();
    }
}
