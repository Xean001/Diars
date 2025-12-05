using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RunaTalento.Models
{
    /// <summary>
    /// Tabla intermedia para relación Muchos a Muchos entre Curso y Docente
    /// </summary>
    public class CursoDocente
    {
        [Key]
        public int IdCursoDocente { get; set; }

        [Required]
        public int IdCurso { get; set; }

        [Required]
        public string IdDocente { get; set; }

        public DateTime FechaAsignacion { get; set; } = DateTime.Now;

        // Navegación
        [ForeignKey("IdCurso")]
        public virtual Curso? Curso { get; set; }

        [ForeignKey("IdDocente")]
        public virtual ApplicationUser? Docente { get; set; }
    }
}
