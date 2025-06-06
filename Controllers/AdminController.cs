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
using SoftWC.Service.Interfaces;
using SoftWC.Service.Implementation;

namespace SoftWC.Controllers;

[Authorize(Roles = "Administrador,Supervisor")]
public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;
    private readonly UserService _userService;
    private readonly SignInManager<Usuario> _signInManager;
    private readonly IExcelExportService _excelExportService;
    private readonly IPdfExportService _pdfExportService;

    private readonly ApplicationDbContext _context;

    public AdminController(ILogger<AdminController> logger, UserService userService, SignInManager<Usuario> signInManager, ApplicationDbContext context, IPdfExportService pdfExportService, IExcelExportService excelExportService)
    {
        _context = context;    
        _userService = userService;
        _logger = logger;
        _signInManager = signInManager;
        _pdfExportService = pdfExportService;
        _excelExportService = excelExportService;
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
                    PrecioBase = g.Key.PrecioBase,
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

    [HttpGet("exportar")]
    public async Task<IActionResult> Exportar(
        int año, int mes, int quincena, string formato)
    {
        var datos = await ObtenerDatosQuincena(año, mes, quincena);
        
        if (formato.ToLower() == "excel")
        {
            var fileContent = _excelExportService.GenerateExcel(datos, año, mes, quincena);
            return File(fileContent, 
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", 
                $"Pagos_{año}_{mes}_Q{quincena}.xlsx");
        }
        else if (formato.ToLower() == "pdf")
        {
            var fileContent = _pdfExportService.GeneratePdf(datos, año, mes, quincena);
            return File(fileContent, "application/pdf", 
                $"Pagos_{año}_{mes}_Q{quincena}.pdf");
        }
        
        return BadRequest("Formato no soportado");
    }

    private async Task<List<ResumenPagoVM>> ObtenerDatosQuincena(int año, int mes, int quincena)
    {
    // Reutiliza la misma lógica del action ResumenPagos
        var (inicio, fin) = CalcularRangoQuincena(año, mes, quincena);

        return await _context.Asistencia
        .Where(a => a.Fecha >= inicio && a.Fecha <= fin)
        .GroupBy(a => new 
        { 
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
            PrecioBase = g.Key.PrecioBase,
            TotalPago = g.Sum(a => a.HorasTrabajadas) * g.Key.PrecioBase,
            Servicios = new List<string> { g.Key.NombreServicio }
            // Detalles se omite o se inicializa como lista vacía si es requerido
        })
        .ToListAsync();
    }
}
