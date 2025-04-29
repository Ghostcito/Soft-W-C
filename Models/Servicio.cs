using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Soft_W_C.Models
{
    public class Servicio
    {
    [Key]
    public int ServicioId { get; set; }
    public string NombreServicio { get; set; }
    public string TipoServicio { get; set; }
    public string Descripcion { get; set; }
    public decimal Precio { get; set; }
    public TimeSpan Duracion { get; set; }
    }
}