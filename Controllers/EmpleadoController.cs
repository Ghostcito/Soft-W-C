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

        public async Task<IActionResult> MarcaEntrada()
        {
            //Validar entrada unica por empleado
            if (!await _asistenciaService.VerificarUnicaEntrada(DateTime.UtcNow.Date))
            {
                return View("EntradaExistente");
            }


            UbicacionDTO ubicacion = new UbicacionDTO
            {
                Latitud = Convert.ToDouble(TempData["Latitud"]),
                Longitud = Convert.ToDouble(TempData["Longitud"]),
                EmpleadoId = TempData["EmpleadoId"]?.ToString()
            };
            var verificacion = await _asistenciaService.ValidarDistancia(ubicacion);
            if (verificacion.Item1 == null)
            {
                return View("NoSedesAsign");
            }
            MarcaViewModel viewModel = new MarcaViewModel
            {
                NombreSede = verificacion.Item1.Nombre_local,
                horaActual = DateTime.Now.ToString("HH:mm"),
                fechaActual = DateTime.Now.ToString("dd/MM/yyyy"),
                localizacionExitosa = verificacion.Item2
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmarEntrada()
        {
            //Obtener Usuario
            var user = await _userService.GetCurrentUserAsync();
            //Generar Asistencia
            Asistencia asistencia = await _asistenciaService.AddEntrada();


            await _asistenciaService.AddAsistencia(asistencia);
            ViewData["HoraRegistrada"] = asistencia.HoraEntrada.Value.ToString("HH:mm");
            ViewData["FechaRegistrada"] = asistencia.Fecha.ToString("dd 'de' MM 'del' yyyy");

            return View("Confirmacion");
        }

        public async Task<IActionResult> MarcaSalida()
        {
            UbicacionDTO ubicacion = new UbicacionDTO
            {
                Latitud = Convert.ToDouble(TempData["Latitud"]),
                Longitud = Convert.ToDouble(TempData["Longitud"]),
                EmpleadoId = TempData["EmpleadoId"]?.ToString()
            };
            var verificacion = await _asistenciaService.ValidarDistancia(ubicacion);
            if (verificacion.Item1 == null)
            {
                return View("NoSedesAsign");
            }
            var asistencia = await _asistenciaService.GetAsistenciaByDate(DateTime.UtcNow.Date);
            if (asistencia == null || asistencia.HoraEntrada == null)
            {
                Console.WriteLine("No se encontró la asistencia para el empleado o no se registró la hora de entrada.");
                return View("NoSalida");
            }
            MarcaViewModel viewModel = new MarcaViewModel
            {
                NombreSede = verificacion.Item1.Nombre_local,
                horaActual = DateTime.Now.ToString("HH:mm"),
                fechaActual = DateTime.Now.ToString("dd/MM/yyyy"),
                HoraEntrada = asistencia.HoraEntrada?.ToString("HH:mm"),
                HorasTrabajadas = await _asistenciaService.CalcularHorasTrabajadas(asistencia.HoraEntrada.Value, DateTime.Now),
                localizacionExitosa = verificacion.Item2
            };

            return View(viewModel);
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmarSalida()
        {
            var asistencia = await _asistenciaService.GetAsistenciaByDate(DateTime.UtcNow.Date);
            if (asistencia == null || asistencia.HoraEntrada == null)
            {
                Console.WriteLine("No se encontró la asistencia para el empleado o no se registró la hora de entrada.");
                return View("Error");
            }
            asistencia = await _asistenciaService.AddSalida(asistencia);
            _asistenciaService.UpdateAsistencia(asistencia);
            ViewData["HoraRegistrada"] = asistencia.HoraSalida?.ToString("HH:mm");
            ViewData["FechaRegistrada"] = asistencia.Fecha.ToString("dd 'de' MM 'del' yyyy");
            return View("Confirmacion");
        }

        public IActionResult PostEntradasSalidas([FromBody] UbicacionDTO ubicacion, string tipo)
        {

            if (ubicacion == null) return BadRequest("No se recibió la ubicación.");
            if (string.IsNullOrEmpty(ubicacion.EmpleadoId)) return BadRequest("No se recibió el ID del empleado.");
            TempData["Latitud"] = ubicacion.Latitud.ToString(System.Globalization.CultureInfo.InvariantCulture);
            TempData["Longitud"] = ubicacion.Longitud.ToString(System.Globalization.CultureInfo.InvariantCulture);
            TempData["EmpleadoId"] = ubicacion.EmpleadoId;

            if (tipo == "salida")
            {
                return Json(new { redirectUrl = Url.Action("MarcaSalida") });
            }
            else
            {
                // Lógica de entrada (por defecto)
                return Json(new { redirectUrl = Url.Action("MarcaEntrada") });
            }
        }

        public async Task<IActionResult> LogoutConfirmado()
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