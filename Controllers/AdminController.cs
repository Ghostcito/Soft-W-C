using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SoftWC.Models;
using SoftWC.Service;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoftWC.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using SoftWC.Models.ViewModels;
using System.Globalization;

namespace SoftWC.Controllers;

[Authorize(Roles = "Administrador,Supervisor")]
public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;
    private readonly UserService _userService;
    private readonly SignInManager<Usuario> _signInManager;

    private readonly ApplicationDbContext _context;

    public AdminController(ILogger<AdminController> logger, UserService userService, SignInManager<Usuario> signInManager, ApplicationDbContext context)
    {
        _context = context;    
        _userService = userService;
        _logger = logger;
        _signInManager = signInManager;
    }

    public IActionResult Index()
    {

        return View();
    }

    public IActionResult Login()
    {
        return View();
    }

    //metodo para verificar el logout
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        TempData["Logout"] = true; // Indicador de que fue una salida voluntaria
        return RedirectToAction("Login", "Account");
    }

    // public async Task<IActionResult> ResumenTareos(int? año, int? mes, int? quincena)
    // {
    //     // Asignar valores por defecto si no se proporcionan
    //     int añoSeleccionado = año ?? DateTime.UtcNow.Year;
    //     int mesSeleccionado = mes ?? DateTime.UtcNow.Month;
    //     int quincenaSeleccionada = (quincena == 1 || quincena == 2) ? quincena.Value : 1;

    //     // Cálculo del rango de fechas
    //     var (inicioQuincena, finQuincena) = CalcularRangoQuincena(añoSeleccionado, mesSeleccionado, quincenaSeleccionada);

    //     // Convertir a UTC
    //     inicioQuincena = DateTime.SpecifyKind(inicioQuincena, DateTimeKind.Utc);
    //     finQuincena = DateTime.SpecifyKind(finQuincena, DateTimeKind.Utc);

    //     // Consulta optimizada que combina datos de Asistencia y Tareo
    //     var resumen = await _context.Asistencia
    //         .AsNoTracking()
    //         .Include(a => a.Empleado)
    //         .Where(a => a.Fecha >= inicioQuincena && a.Fecha <= finQuincena)
    //         .GroupBy(a => new { a.IdEmpleado, a.Empleado.UserName, a.Empleado.DNI })
    //         .Select(g => new ResumenTareoVM
    //         {
    //             Empleado = new Usuario
    //             {
    //                 Id = g.Key.IdEmpleado,
    //                 UserName = g.Key.UserName,
    //                 DNI = g.Key.DNI
    //             },
    //             TotalHoras = g.Sum(a => a.HorasTrabajadas),
    //             TotalPago = g.Sum(a => a.HorasTrabajadas * 
    //                 _context.Tareo
    //                     .Where(t => t.IdEmpleado == g.Key.IdEmpleado && 
    //                             t.Fecha >= inicioQuincena && 
    //                             t.Fecha <= finQuincena)
    //                     .Average(t => t.PagoPorHora)), // Obtener el promedio de pago por hora
    //             Detalles = _context.Tareo
    //                 .Where(t => t.IdEmpleado == g.Key.IdEmpleado && 
    //                         t.Fecha >= inicioQuincena && 
    //                         t.Fecha <= finQuincena)
    //                 .OrderBy(t => t.Fecha)
    //                 .ToList()
    //         })
    //         .ToListAsync();

    //     // Pasar valores a ViewBag para mantener filtros seleccionados
    //     ViewBag.AñoSeleccionado = añoSeleccionado;
    //     ViewBag.MesSeleccionado = mesSeleccionado;
    //     ViewBag.QuincenaSeleccionada = quincenaSeleccionada;
    //     ViewBag.Meses = Enumerable.Range(1, 12).Select(m => new SelectListItem
    //     {
    //         Value = m.ToString(),
    //         Text = CultureInfo.GetCultureInfo("es-ES").DateTimeFormat.GetMonthName(m)
    //     }).ToList();

    //     return View(resumen);
    // }

    private (DateTime inicio, DateTime fin) CalcularRangoQuincena(int año, int mes, int quincena)
    {
        DateTime inicio, fin;
    
        if (quincena == 1)
        {
            inicio = new DateTime(año, mes, 1, 0, 0, 0, DateTimeKind.Utc);
            fin = new DateTime(año, mes, 15, 23, 59, 59, DateTimeKind.Utc);
        }
        else
        {
            inicio = new DateTime(año, mes, 16, 0, 0, 0, DateTimeKind.Utc);
            fin = new DateTime(año, mes, DateTime.DaysInMonth(año, mes), 23, 59, 59, DateTimeKind.Utc);
        }
        
        return (inicio, fin);
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [HttpGet]
    public async Task<IActionResult> ResumenPagos(int? año = null, int? mes = null, int? quincena = null)
    {
        try
        {
            // Establecer valores por defecto
            año = año ?? DateTime.Now.Year;
            mes = mes ?? DateTime.Now.Month;
            quincena = quincena ?? (DateTime.Now.Day <= 15 ? 1 : 2);

            // Validar quincena
            if (quincena != 1 && quincena != 2)
            {
                quincena = DateTime.Now.Day <= 15 ? 1 : 2;
            }

            var (inicio, fin) = CalcularRangoQuincena(año.Value, mes.Value, quincena.Value);
            
            // Consulta optimizada para agrupar correctamente
            var resumen = await _context.Asistencia
                .Where(a => a.Fecha >= inicio && a.Fecha <= fin)
                .GroupBy(a => new { 
                    a.IdEmpleado,
                    a.Empleado.UserName,
                    a.Empleado.DNI,
                    a.Empleado.Servicio.NombreServicio,
                    a.Empleado.Servicio.PrecioBase
                })
                .Select(g => new ResumenPagoVM
                {
                    Empleado = g.Key.UserName,
                    DNI = g.Key.DNI,
                    TotalHoras = g.Sum(a => a.HorasTrabajadas),
                    TotalPago = g.Sum(a => a.HorasTrabajadas) * g.Key.PrecioBase,
                    Servicios = new List<string> { g.Key.NombreServicio },
                    Detalles = g.Select(a => new DetallePagoVM
                    {
                        Fecha = a.Fecha,
                        Servicio = g.Key.NombreServicio,
                        Horas = a.HorasTrabajadas,
                        PagoHora = g.Key.PrecioBase,
                        TotalDia = a.HorasTrabajadas * g.Key.PrecioBase
                    }).ToList()
                }).ToListAsync();
            
            ViewBag.AñoSeleccionado = año;
            ViewBag.MesSeleccionado = mes;
            ViewBag.QuincenaSeleccionada = quincena;
            
            return View(resumen);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al generar el resumen de pagos");
            return View(new List<ResumenPagoVM>());
        }
    }
}
