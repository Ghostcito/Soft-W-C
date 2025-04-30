using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Soft_W_C.Models
{
    public class Salida_Temporal
    {
        [Key]
        public int IdSalidaTemporal { get; set; }
        public Usuario IdEmpleado { get; set; }
        public Asistencia IdAsistencia { get; set; }
        public TimeSpan HoraSalida { get; set; }
        public TimeSpan HoraRetorno { get; set; }
        public string Motivo { get; set; }
        public string Estado { get; set; } // Aprobada, Rechazada, Pendiente



    }
}