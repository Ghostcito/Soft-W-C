using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoftWC.Models
{
    public class UsuarioTurno
    {
        [Key]
        public int UsuarioTurnoId { get; set; }

        [Required]
        public string UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }

        [Required]
        public int TurnoId { get; set; }

        [ForeignKey("TurnoId")]
        public Turno Turno { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; } // Null si es el turno actual

        public bool Activo { get; set; } = true;
    }
}