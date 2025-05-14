using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace SoftWC.Models
{
    //ya combinamos empleados y administrativos en un solo modelo
    public class Usuario : IdentityUser
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }

        [Column(TypeName = "nvarchar(8)")]
        public string? DNI { get; set; }
        public Sede? IdSede { get; set; }
        public DateTime? FechaIngreso { get; set; } // fecha de ingreso a la empresa
        public DateTime? FechaNacimiento { get; set; }
        public string? NivelAcceso { get; set; }
        public string? Estado { get; set; } //activo o inactivo
        public decimal? Salario { get; set; }

        //RELACIONES EMPLEADO - SUPERVISOR
        public ICollection<Supervision> EmpleadosSupervisados { get; set; } // Si es Supervisor
        public ICollection<Supervision> SupervisoresAsignados { get; set; } // Si es Empleado
    }
}