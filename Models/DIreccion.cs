using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Soft_W_C.Models
{
    public class DIreccion
    {
    [Key]
    public int DireccionId { get; set; }
    public int ClienteId { get; set; }
    public string DireccionCalle { get; set; }
    public string Ciudad { get; set; }
    public string Provincia { get; set; }
    public string Pais { get; set; }
    public decimal Latitud { get; set; }
    public decimal Longitud { get; set; }
    }
}