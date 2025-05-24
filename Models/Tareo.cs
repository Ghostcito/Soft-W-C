using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using SoftWC.Models.ModelEnum;

namespace SoftWC.Models
{
    public class Tareo
{
    [Key]
    public int IdTareo { get; set; }

    [Required]
    public string IdEmpleado { get; set; }

    [ForeignKey("IdEmpleado")]
    public Usuario Empleado { get; set; }

    [Required]
    public DateTime Fecha { get; set; }

    // Relación con servicio
    public int ServicioId { get; set; }
    [ForeignKey("ServicioId")]
    public Servicio Servicio { get; set; }

    [Column(TypeName = "decimal(5,2)")]
    public decimal HorasTrabajadas { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal PagoPorHora { get; set; }

    [Column(TypeName = "decimal(10,2)")]
    public decimal TotalGanado { get; set; }

    public string? Observacion { get; set; }

    // Relación con turno
    public int? TurnoId { get; set; }
    [ForeignKey("TurnoId")]
    public Turno? Turno { get; set; }

    // Estado del tareo
    public EstadoTareo Estado { get; set; } = EstadoTareo.Pendiente;
}
}