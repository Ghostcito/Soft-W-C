using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SoftWC.Models
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

        [Display(Name = "Empleados asignados")]
        public Usuario Empleado { get; set; }

        [Display(Name = "Fecha Inicio actividades")]
        public DateTime FechaInicio { get; set; }

        [Display(Name = "Fecha de fin actividades")]
        public DateTime? FechaFin { get; set; } // NULL si sigue activo
    }
}