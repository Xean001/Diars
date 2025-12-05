using System.ComponentModel.DataAnnotations;

namespace RunaTalento.Models
{
    /// <summary>
    /// Representa un incentivo que se otorga automáticamente al alcanzar ciertos puntos
    /// </summary>
    public class Incentivo
    {
        [Key]
        public int IdIncentivo { get; set; }

        [Required(ErrorMessage = "El nombre del incentivo es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(500)]
        public string? Descripcion { get; set; }

        [Required(ErrorMessage = "Los puntos requeridos son obligatorios")]
        [Range(1, 999999, ErrorMessage = "Los puntos deben estar entre 1 y 999999")]
        public int PuntosRequeridos { get; set; }

        [StringLength(500)]
        public string? IconoUrl { get; set; }

        /// <summary>
        /// Indica si el incentivo está activo y se otorga automáticamente
        /// </summary>
        public bool Activo { get; set; } = true;

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        // Navegación hacia estudiantes que han obtenido este incentivo
        public virtual ICollection<IncentivoEstudiante>? IncentivoEstudiantes { get; set; }
    }
}
