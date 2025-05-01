using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Soft_W_C.Service;

namespace Soft_W_C.Controllers
{
    public class EmpleadoController : Controller
    {
        private readonly ILogger<EmpleadoController> _logger;
        private readonly UserService _userService;

        public EmpleadoController(ILogger<EmpleadoController> logger, UserService userService)
        {
            _userService = userService;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

       public async Task<IActionResult> AllUsers()
        {
            var empleados = _userService.GetAllUsers();
            return View("Index", empleados); // Pasa los empleados a la vista
        }

        [HttpGet("Marca")]
        public IActionResult Marca()
        {
            return View("Marca"); // Asegúrate de que la vista Marca exista en Views/Empleado/
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}