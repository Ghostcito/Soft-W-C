using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Soft_W_C.Models
{
    public class Asistencia
    {
        [Key]
        public int IdAsistencia { get; set; }

        public string? IdEmpleado { get; set; } // Tipo string porque IdentityUser usa string para el Id

        [ForeignKey("IdEmpleado")]
        public Usuario Empleado { get; set; } //para la navegacion
        public DateTime Fecha { get; set; }
        public DateTime? HoraEntrada { get; set; }
        public DateTime? HoraSalida { get; set; }
        public decimal HorasTrabajadas { get; set; }
        //Podris tambien acotar las horas descontadas
        //public decimal Horas Descontadas
        public bool Presente { get; set; } // bool para validar y dar una representacion grafica

        //observacion para ser aumentada en el dashboard por parte de un supervisor o jefe de area
        public string? Observacion { get; set; }


    }
}