using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Soft_W_C.Models
{
    public class Cliente
    {
    [Key]
    public int ClienteId { get; set; }
    public string Nombre { get; set; }
    public string Apellido { get; set; }
    public string Telefono { get; set; }
    public string Correo { get; set; }
    public string TipoCliente { get; set; }
    public string Estado { get; set; }
    public DateTime UltimaModificacion { get; set; }
    }
}