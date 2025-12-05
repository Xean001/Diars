using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RunaTalento.Models
{
    /// <summary>
    /// Tabla intermedia: Incentivos otorgados a estudiantes
    /// </summary>
    public class IncentivoEstudiante
    {
        [Key]
        public int IdIncentivoEstudiante { get; set; }

        [Required]
        public int IdIncentivo { get; set; }

        [Required]
        public string IdEstudiante { get; set; }

        public DateTime FechaOtorgado { get; set; } = DateTime.Now;

        /// <summary>
        /// Puntaje que tenía el estudiante cuando obtuvo el incentivo
        /// </summary>
        public int PuntajeAlObtener { get; set; }

        // Navegación
        [ForeignKey("IdIncentivo")]
        public virtual Incentivo? Incentivo { get; set; }

        [ForeignKey("IdEstudiante")]
        public virtual ApplicationUser? Estudiante { get; set; }
    }
}
