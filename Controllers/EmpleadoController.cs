using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Soft_W_C.Models;
using Soft_W_C.Service;
using Soft_W_C.ViewModel;

namespace Soft_W_C.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly ILogger<EmpleadoController> _logger;
        private readonly UserService _userService;
        private readonly AsistenciaService _asistenciaService;
        private readonly EmpleadoService _empleadoService;

        public EmpleadoController(ILogger<EmpleadoController> logger, UserService userService, AsistenciaService asistenciaService, EmpleadoService empleadoService)
        {
            _asistenciaService = asistenciaService;
            _logger = logger;
            _userService = userService;
            _empleadoService = empleadoService;
        }

        public IActionResult Index()
        {
            List<Usuario> usuarios = _empleadoService.GetEmpleados().Result;
            return View(usuarios);
        }


        public IActionResult MarcaEntrada()
        {
            Asistencia asis = _asistenciaService.AddEntrada().Result;
            var viewModel = new MarcaViewModel
            {
                asistencia = asis,
                usuario = _userService.GetCurrentUserAsync().Result
            };
            return View("Marca", viewModel);
        }

        public IActionResult MarcaSalida()
        {
            Asistencia asis = _asistenciaService.AddSalida().Result;
            if (asis != null)
            {
                _asistenciaService.CalcularHorasTrabajadas(asis.IdAsistencia);
                var viewModel = new MarcaViewModel
                {
                    asistencia = asis,
                    usuario = _userService.GetCurrentUserAsync().Result
                };
                return View("Marca", viewModel);
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