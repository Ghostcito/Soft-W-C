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

    [Required]
    [StringLength(100)]
    public string NombreServicio { get; set; }

    [Required]
    public string TipoServicio { get; set; }

    [Required]
    public string Descripcion { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal PrecioBase { get; set; }

    // Duración estimada del servicio
    public TimeSpan? DuracionEstimada { get; set; }
}
}