using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SoftWC.Models
{
    //cliente para manejarlo como sede
    public class Cliente
    {
        [Key]
        public int ClienteId { get; set; }
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string? Telefono { get; set; }
        public string? Correo { get; set; }

        public TipoClienteEnum TipoCliente { get; set; }

        //Estado para que seria? podria ser Activo, Inactivo, Bloqueado, etc. pero no lo veo muy significativo
        public bool Estado { get; set; } // activo o inactivo    
    }
}