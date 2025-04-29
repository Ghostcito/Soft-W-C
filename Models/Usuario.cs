using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Soft_W_C.Models
{
    public class Usuario : IdentityUser
    {
    public string? Nombre { get; set; }
    public string? Apellido { get; set; }
    public string? DNI { get; set; }  
    public DateTime? FechaIngreso { get; set; }
    public DateTime? FechaNacimiento { get; set; }
    public string? NivelAcceso { get; set; }
    public string? Estado { get; set; }
    public decimal? Salario { get; set; }
    }
}