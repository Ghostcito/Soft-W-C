using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Soft_W_C.Models
{
    public class Tareo
    {
    [Key]
    public int IdTareo { get; set; }
    public Usuario IdEmpleado { get; set; }
    public DateTime Fecha { get; set; }
    public string Descripcion { get; set; }//Nose creo que no tendria descripción
    public decimal HorasTrabajadas { get; set; }
    public decimal PagoPorHora { get; set; }
    public decimal TotalGanado { get; set; }
    }
}