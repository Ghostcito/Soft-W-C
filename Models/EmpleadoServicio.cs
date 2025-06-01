using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SoftWC.Models
{
    public class EmpleadoServicio
    {
        public string EmpleadoId { get; set; }
        public Usuario Empleado { get; set; }

        public int ServicioId { get; set; }
        public Servicio Servicio { get; set; }

        //En un caso hipotitetico podria servir

        //public decimal TarifaPorHora { get; set; }  
    }
}