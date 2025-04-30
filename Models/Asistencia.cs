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
    public long IdAsistencia { get; set; }
    public Usuario? IdEmpleado { get; set; }
    public DateTime Fecha { get; set; }
    public TimeSpan HoraEntrada { get; set; }
    public TimeSpan HoraSalida { get; set; }
    public decimal HorasTrabajadas {get;set;}
    //Podris tambien acotar las horas descontadas
    //public decimal Horas Descontadas
    public bool Presente { get; set; }

    //Este dudo que se use, pero lo dejo por si acaso
    public string Observacion { get; set; }
    }
}