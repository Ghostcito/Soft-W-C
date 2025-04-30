using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Soft_W_C.Models
{
    /*Hace falta un entidad para manejar las sedes*/
    public class Sede
    {
        [Key]
        public long IdSede {get;set;}
        //este atributo no es necesario, ya que el nombre de la propidad será el mismo
        //public string? Nombre {get;set;}
        public DIreccion? IdDireccion {get;set;}
        [Column(TypeName = "nvarchar(20)")]
        public SedeEnum estadoSede {get;set;}





    }
}