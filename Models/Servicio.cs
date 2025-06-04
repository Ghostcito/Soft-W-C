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
    [Display(Name = "Nombre de servicio")]
    public string NombreServicio { get; set; }

    [Required]
    [Display(Name = "Tipo de servicio")]
    public string TipoServicio { get; set; }

    [Required]
    [Display(Name = "Descripción")]
    public string Descripcion { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    [Display(Name = "Precio base por hora de trabajo")]
    public decimal PrecioBase { get; set; }

    // Duración estimada del servicio
    [Display(Name = "Duración estimada")]
    public TimeSpan? DuracionEstimada { get; set; }
}
}