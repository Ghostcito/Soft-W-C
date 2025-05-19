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
            var user = _userService.GetCurrentUserAsync().Result;
            //Console.WriteLine($"Usuario: {user.Nombre} {user.Apellido}, DNI: {user.DNI}, ID: {user.Id}");

            // if (empleado == null || empleado.Result.Sede == null)

            //     return BadRequest("Empleado o sede no encontrados.");

            //     var distancia = GeoUtils.CalcularDistancia(
            //     empleado.Result.Sede.Latitud,
            //     empleado.Result.Sede.Longitud,
            //     ubicacion.Latitud,
            //     ubicacion.Longitud
            // );

            // if (distancia > empleado.Result.Sede.RadioValidacion)
            // {
            //     Console.WriteLine($"Distancia: {distancia} metros, FUERA DEL RANGO");
            //     return BadRequest("Estás fuera del rango permitido para marcar asistencia.");
            // }
            // Console.WriteLine($"Distancia: {distancia} metros, dentro del rango");

            // // Aquí podrías comparar con la ubicación de la sede o simplemente guardar en BD.
            // // Console.WriteLine($"Ubicación recibida: {ubicacion.Latitud}, {ubicacion.Longitud}, {ubicacion.EmpleadoId}");
            // return Ok($"Ubicación recibida: Latitud {ubicacion.Latitud}, Longitud {ubicacion.Longitud}, EmpleadoId {ubicacion.EmpleadoId}, ASISTENCIA REGISTRADA");
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

        public IActionResult MarcarEntrada([FromBody] UbicacionDTO ubicacion)
        {
            Asistencia asis = _asistenciaService.AddEntrada().Result;
            var viewModel = new MarcaViewModel
            {
                asistencia = asis,
                usuario = _userService.GetCurrentUserAsync().Result,
                verificacion = _asistenciaService.ValidarDistancia(ubicacion).Result

            };
            return View("Marca", viewModel);
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