using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Soft_W_C.Models
{
    // basada en indicadores para la evaluacion de los empleados
    public class Evaluaciones
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdEvaluacion { get; set; }
        public string IdEmpleado { get; set; } // Tipo string porque IdentityUser usa string para el Id

        [ForeignKey("IdEmpleado")]
        public Usuario Empleado { get; set; }
        public string TipoEmpleado { get; set; } // Tipo de empleado (Operario de limpieza)
        public DateTime FechaEvaluacion { get; set; }
        public string Descripcion { get; set; } // Descripción de la evaluación

            // --- INDICADORES DE EVALUACIÓN --- //
        public Calificacion Responsabilidad { get; set; }

        public Calificacion Puntualidad { get; set; }

        public Calificacion CalidadTrabajo { get; set; }

        public Calificacion UsoMateriales { get; set; }

        public Calificacion Actitud { get; set; }

        // Calificación global calculada
        
        public enum Calificacion
        {
            [Display(Name = "Deficiente")]
            Deficiente = 1,
            
            [Display(Name = "Regular")]
            Regular = 2,
            
            [Display(Name = "Bueno")]
            Bueno = 3,
            
            [Display(Name = "Excelente")]
            Excelente = 4
        }


    }
}