using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SoftWC.Models
{
    public class Sede
    {
        [Key]
        public int SedeId { get; set; }
        public int ClienteId { get; set; }

        [ForeignKey("ClienteId")]
        public Cliente Cliente { get; set; }

        public string? Nombre_local { get; set; } //para que puedan ubicar en caso la empresa tenga un local con nombre
        public string? Direccion_local { get; set; }
        public string? Ciudad { get; set; }
        public string? Provincia { get; set; }
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public decimal Radio { get; set; } //radio de la sede
        public SedeEnum estadoSede { get; set; }

        // public string? Pais { get; set; } se borra pais por que trabaja solo en PERU
    }
}