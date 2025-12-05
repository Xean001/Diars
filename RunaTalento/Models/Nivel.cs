using System.ComponentModel.DataAnnotations;

namespace RunaTalento.Models
{
    /// <summary>
    /// Representa los niveles de logro de los estudiantes
    /// </summary>
    public class Nivel
    {
        [Key]
        public int IdNivel { get; set; }

        [Required(ErrorMessage = "El nombre del nivel es obligatorio")]
        [StringLength(50)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El puntaje mínimo es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "El puntaje mínimo debe ser positivo")]
        public int PuntajeMinimo { get; set; }

        [Required(ErrorMessage = "El puntaje máximo es obligatorio")]
        [Range(0, int.MaxValue, ErrorMessage = "El puntaje máximo debe ser positivo")]
        public int PuntajeMaximo { get; set; }
    }
}
