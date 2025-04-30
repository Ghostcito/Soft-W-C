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
    public long ClienteId { get; set; }
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public string? Telefono { get; set; }
    public string? Correo { get; set; }
    [Column(TypeName = "nvarchar(120)")]
    public TipoClienteEnum TipoCliente { get; set; }
    //Estado para que seria? podria ser Activo, Inactivo, Bloqueado, etc. pero no lo veo muy significativo
    public string Estado { get; set; }
    public DateTime UltimaModificacion { get; set; }
    }
}