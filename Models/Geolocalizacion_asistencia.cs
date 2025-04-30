using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Soft_W_C.Models
{
    public class Geolocalizacion_asistencia
    {
    [Key]
    public int IdGeolocalizacion { get; set; }
    public Asistencia IdAsistencia { get; set; }
    public decimal Latitud { get; set; }
    public decimal Longitud { get; set; }
    public decimal DistanciaMetros { get; set; }
    public string EstadoValidacion { get; set; }
    public DateTime Timestamp { get; set; } //TIMESTAMP  para que sirve?
    }
}