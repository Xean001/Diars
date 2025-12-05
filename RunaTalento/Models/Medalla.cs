using System.ComponentModel.DataAnnotations;

namespace RunaTalento.Models
{
    /// <summary>
    /// Representa una medalla o insignia que pueden ganar los estudiantes
    /// </summary>
    public class Medalla
    {
        [Key]
        public int IdMedalla { get; set; }

        [Required(ErrorMessage = "El nombre de la medalla es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(255)]
        public string? Descripcion { get; set; }

        [StringLength(255)]
        public string? ImagenUrl { get; set; }

        // Navegación hacia las medallas otorgadas a estudiantes
        public virtual ICollection<MedallaEstudiante>? MedallasEstudiantes { get; set; }
    }
}
