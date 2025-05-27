using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftWC.Models
{
    public class Turno
    {
        [Key]
        public int TurnoId { get; set; }

        [Required]
        [Display(Name = "Tipo de turno")]
        public string NombreTurno { get; set; } // Ej: "Mañana", "Tarde", "Noche"

        [Required]
        [Display(Name = "Hora de inicio")]
        public TimeSpan HoraInicio { get; set; }

        [Required]
        [Display(Name = "Hora de fin")]
        public TimeSpan HoraFin { get; set; }

        public string Descripcion { get; set; }

        // Relación con usuarios (empleados)
        public ICollection<UsuarioTurno> UsuarioTurnos { get; set; } = new List<UsuarioTurno>();

    }
}