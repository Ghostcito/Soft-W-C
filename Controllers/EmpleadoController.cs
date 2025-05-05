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
            var empleados = _userService.GetAllUsersByRol().Result;
            return View(empleados);
        }

        //    public async Task<IActionResult> AllUsers()
        //     {
        //         var empleados = _userService.GetAllUsers();
        //         return View("Index", empleados);
        //     }

        [HttpGet("Marca")]
        public IActionResult Marca()
        {
            return View("Marca");
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