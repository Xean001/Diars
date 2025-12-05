using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace RunaTalento.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nombres { get; set; }
        public string Apellidos { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public int PuntajeTotal { get; set; } = 0;
        public string Estado { get; set; } = "Activo";
        public int? IdNivel { get; set; }

        // Navegación al nivel
        [ForeignKey("IdNivel")]
        public virtual Nivel? Nivel { get; set; }
    }
}
