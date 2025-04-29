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
    public int IdEmpleado { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan HoraEntrada { get; set; }
    public TimeSpan HoraSalida { get; set; }
    public bool Presente { get; set; }
    public string Observacion { get; set; }
    }
}