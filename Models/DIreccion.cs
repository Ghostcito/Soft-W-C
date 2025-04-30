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
    public long DireccionId { get; set; }
    public Cliente? ClienteId { get; set; }
    /*La cambio por Descripción antes fue Dirección Calle*/
    public string? Descripción { get; set; }
    public string? Ciudad { get; set; }
    public string? Provincia { get; set; }
    public string? Pais { get; set; }
    public decimal Latitud { get; set; }
    public decimal Longitud { get; set; }
    }
}