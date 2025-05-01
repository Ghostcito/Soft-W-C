using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Soft_W_C.Models
{

    //PARA PERMISOS EN CASO DE QUE UN EMPLEADO TENGA QUE SALIR POR UN TIEMPO Y LUEGO RETORNAR
    //FUNCIONALIDAD PARA FUTURO
    public class Permisos
    {
        [Key]
        public int IdPermiso { get; set; }
        public string IdEmpleado { get; set; }

        [ForeignKey("IdEmpleado")]
        public Usuario Empleado { get; set; }

        // public Asistencia IdAsistencia { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public TimeSpan HoraRetorno { get; set; }
        public string Motivo { get; set; }
        public string Estado { get; set; } // Aprobada, Rechazada, Pendiente

    }
}