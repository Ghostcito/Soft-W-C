using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SoftWC.Models
{
    // basada en indicadores para la evaluacion de los empleados
   public class Evaluaciones  // ✔ Nombre en singular
{
    [Key]
    public int IdEvaluacion { get; set; }

    [Required]
    public string IdEmpleado { get; set; }

    [ForeignKey("IdEmpleado")]
    public Usuario Empleado { get; set; }

    [Required]
    public string TipoEmpleado { get; set; } // Ej: "Limpieza", "Supervisor"

    [Required]
    public DateTime FechaEvaluacion { get; set; }

    public string? Descripcion { get; set; }

    // --- Indicadores de evaluación (usando el enum Calificacion) --- //
    public Calificacion Responsabilidad { get; set; }
    public Calificacion Puntualidad { get; set; }
    public Calificacion CalidadTrabajo { get; set; }
    public Calificacion UsoMateriales { get; set; }
    public Calificacion Actitud { get; set; }

    // Calificación global calculada (no se almacena en DB)
    [NotMapped]
    public decimal PuntajeGlobal => 
        ((int)Responsabilidad + (int)Puntualidad + (int)CalidadTrabajo + 
        (int)UsoMateriales + (int)Actitud) / 5.0m;

    // Relación con el supervisor que evaluó
    public string? EvaluadorId { get; set; }
    [ForeignKey("EvaluadorId")]
    public Usuario? Evaluador { get; set; }
}

public enum Calificacion
{
    [Display(Name = "Deficiente")] Deficiente = 1,
    [Display(Name = "Regular")] Regular = 2,
    [Display(Name = "Bueno")] Bueno = 3,
    [Display(Name = "Excelente")] Excelente = 4
}
}