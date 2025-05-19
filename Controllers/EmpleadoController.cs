using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SoftWC.Models;
using SoftWC.Service;
using SoftWC.ViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SoftWC.Models.Dto;

namespace SoftWC.Controllers
{
    [Authorize]
    public class EmpleadoController : Controller
    {
        private readonly ILogger<EmpleadoController> _logger;
        private readonly UserService _userService;
        private readonly AsistenciaService _asistenciaService;
        private readonly EmpleadoService _empleadoService;
        private readonly SignInManager<Usuario> _signInManager;


        public EmpleadoController(ILogger<EmpleadoController> logger, UserService userService, AsistenciaService asistenciaService, EmpleadoService empleadoService, SignInManager<Usuario> signInManager)
        {
            _asistenciaService = asistenciaService;
            _logger = logger;
            _userService = userService;
            _empleadoService = empleadoService;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult MarcaAsistencia()
        {
            //Obtener Usuario
            var user = _userService.GetCurrentUserAsync().Result;
            //Generar Asistencia



            return Redirect("/Empleado/MarcaEntrada");
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

        public IActionResult MarcaEntrada()
        {
            UbicacionDTO ubicacion = new UbicacionDTO
            {
                Latitud = Convert.ToDouble(TempData["Latitud"]),
                Longitud = Convert.ToDouble(TempData["Longitud"]),
                EmpleadoId = TempData["EmpleadoId"]?.ToString()
            };
            Asistencia asis = _asistenciaService.AddEntrada().Result;
            var viewModel = new MarcaViewModel
            {
                asistencia = asis,
                usuario = _userService.GetCurrentUserAsync().Result,
                verificacion = _asistenciaService.ValidarDistancia(ubicacion).Result

            };

            return View(viewModel);
        }

        public IActionResult PostEntrada([FromBody] UbicacionDTO ubicacion)
        {
            if (ubicacion == null) return BadRequest("No se recibió la ubicación.");
            if (string.IsNullOrEmpty(ubicacion.EmpleadoId)) return BadRequest("No se recibió el ID del empleado.");
            TempData["Latitud"] = ubicacion.Latitud.ToString(System.Globalization.CultureInfo.InvariantCulture);
            TempData["Longitud"] = ubicacion.Longitud.ToString(System.Globalization.CultureInfo.InvariantCulture);
            TempData["EmpleadoId"] = ubicacion.EmpleadoId;
            return Json(new { redirectUrl = Url.Action("MarcaEntrada") }); ;
        }

        public IActionResult LogoutConfirmado()
        {
            return View();
        }

        //metodo para verificar el logout
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["LogoutSuccess"] = true; // Se guarda en TempData
            return Redirect("~/Identity/Account/Login");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}