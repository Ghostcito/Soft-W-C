using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Soft_W_C.Models;
using Soft_W_C.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Soft_W_C.Controllers;

[Authorize(Roles = "Administrador,Supervisor")]
public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;
    private readonly UserService _userService;
    private readonly SignInManager<Usuario> _signInManager;

    public AdminController(ILogger<AdminController> logger, UserService userService, SignInManager<Usuario> signInManager)
    {
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

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
