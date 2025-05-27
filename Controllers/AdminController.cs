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

    public async Task<IActionResult> ResumenTareos(int? año, int? mes, int? quincena)
    {
        // Asignar valores por defecto si no se proporcionan
        int añoSeleccionado = año ?? DateTime.UtcNow.Year;
        int mesSeleccionado = mes ?? DateTime.UtcNow.Month;
        int quincenaSeleccionada = (quincena == 1 || quincena == 2) ? quincena.Value : 1;

        // Cálculo del rango de fechas
        var (inicioQuincena, finQuincena) = CalcularRangoQuincena(añoSeleccionado, mesSeleccionado, quincenaSeleccionada);

        // Convertir a UTC si tienen Kind = Unspecified
        inicioQuincena = DateTime.SpecifyKind(inicioQuincena, DateTimeKind.Utc);
        finQuincena = DateTime.SpecifyKind(finQuincena, DateTimeKind.Utc);

        // Obtener los tareos en el rango de fechas
        var tareos = await _context.Tareo
            .AsNoTracking()
            .Include(t => t.Empleado)
            .Include(t => t.Servicio)
            .Where(t => t.Fecha >= inicioQuincena && t.Fecha <= finQuincena)
            .OrderBy(t => t.Fecha)
            .ThenBy(t => t.Empleado.Apellido)
            .ToListAsync();

        // Agrupar y transformar a ViewModel
        var resumen = tareos
            .GroupBy(t => t.Empleado)
            .Select(g => new ResumenTareoVM
            {
                Empleado = g.Key,
                TotalHoras = g.Sum(t => t.HorasTrabajadas),
                TotalPago = g.Sum(t => t.TotalGanado),
                Detalles = g.OrderBy(t => t.Fecha).ToList()
            }).ToList();

        // Pasar valores a ViewBag para mantener filtros seleccionados
        ViewBag.AñoSeleccionado = añoSeleccionado;
        ViewBag.MesSeleccionado = mesSeleccionado;
        ViewBag.QuincenaSeleccionada = quincenaSeleccionada;
        ViewBag.Meses = Enumerable.Range(1, 12).Select(m => new SelectListItem
        {
            Value = m.ToString(),
            Text = new DateTime(1, m, 1).ToString("MMMM", new System.Globalization.CultureInfo("es-ES"))
        }).ToList();

        return View(resumen);
    }

    private (DateTime inicio, DateTime fin) CalcularRangoQuincena(int año, int mes, int quincena)
    {
        if (quincena != 1 && quincena != 2)
            throw new ArgumentException("Quincena debe ser 1 o 2");

        var inicio = (quincena == 1)
            ? new DateTime(año, mes, 1)
            : new DateTime(año, mes, 16);

        var finDia = (quincena == 1) ? 15 : DateTime.DaysInMonth(año, mes);
        var fin = new DateTime(año, mes, finDia, 23, 59, 59);

        return (inicio, fin);
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
