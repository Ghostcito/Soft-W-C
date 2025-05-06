using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Soft_W_C.Models;
using Soft_W_C.Service;

namespace Soft_W_C.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly ILogger<EmpleadoController> _logger;
        private readonly UserService _userService;
        private readonly AsistenciaService _asistenciaService;

        public EmpleadoController(ILogger<EmpleadoController> logger, UserService userService, AsistenciaService asistenciaService)
        {
            _asistenciaService = asistenciaService;
            _logger = logger;
            _userService = userService;
        }

        public IActionResult Index()
        {
            Usuario usuario = _userService.GetCurrentUserAsync().Result;
            return View(usuario);
        }


        public IActionResult MarcaEntrada()
        {
            Asistencia asis = _asistenciaService.AddEntrada().Result;
            return View("Marca", asis);
        }

        public IActionResult MarcaSalida()
        {
            Asistencia asis = _asistenciaService.AddSalida().Result;
            if (asis != null)
            {
                _asistenciaService.CalcularHorasTrabajadas(asis.IdAsistencia);
                return View("Marca");
            }
            else
            {
                Console.WriteLine("No se encontró la asistencia para el empleado o no se registró la hora de entrada.");

                return View("Error");
            }

        }

        [HttpGet("Confirmacion")]
        public IActionResult Confirmacion()
        {
            return View("Confirmacion");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}