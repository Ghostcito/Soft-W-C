using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Soft_W_C.Models
{
    public class Evaluacion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdEvaluacion { get; set; }
        public Usuario IdEmpleado { get; set; }
        public string TipoEmpleado { get; set; } // Tipo de empleado (Ej: Operario, Administrativo, etc.)
        public DateTime FechaEvaluacion { get; set; }
        public string Descripcion { get; set; } // Descripción de la evaluación

        /// Calificaciones - Hay que ver el tema de indicadores mas adelante

    }
}