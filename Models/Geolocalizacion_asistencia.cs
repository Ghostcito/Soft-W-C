using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SoftWC.Models
{
    public class Geolocalizacion_asistencia //guardar intentos de geolocalizacion con asistencia
    {
    [Key]
    public int IdGeolocalizacion { get; set; }
    public int IdAsistencia { get; set; }

    [ForeignKey("IdAsistencia")]
    public Asistencia Asistencia { get; set; }
    public decimal Latitud { get; set; }
    public decimal Longitud { get; set; }
    public decimal DistanciaMetros { get; set; } // validar 12 metros para que se valida la asistencia
    public string EstadoValidacion { get; set; } // true o false
    public DateTime Timestamp { get; set; } //TIMESTAMP -> para la hora automatica de la geolocalizacion
    }
}