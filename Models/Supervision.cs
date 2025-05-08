using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Soft_W_C.Models
{
    public class Supervision
    {
        [Key]
        public int Id { get; set; }
        public string SupervisorId { get; set; }
        [ForeignKey("SupervisorId")]
        public Usuario Supervisor { get; set; }

        public string EmpleadoId { get; set; }
        [ForeignKey("EmpleadoId")]
        public Usuario Empleado { get; set; }

        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; } // NULL si sigue activo
    }
}