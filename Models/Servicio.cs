using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SoftWC.Models
{
    public class Servicio
    {
    [Key]
    public int ServicioId { get; set; }
    public string NombreServicio { get; set; }
    public string TipoServicio { get; set; } //limpieza de ambientes, fumigacion integral y limpieza en eventos
    public string Descripcion { get; set; }
    public decimal Precio { get; set; }
    public TimeSpan Duracion { get; set; }
    }

    // comprobar si hay cambio de pago de acuedo tipo de servicio
}