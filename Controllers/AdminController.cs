using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Soft_W_C.Models;
using Soft_W_C.Service;

namespace Soft_W_C.Controllers;

public class AdminController : Controller
{
    private readonly ILogger<AdminController> _logger;
    private readonly UserService _userService;

    public AdminController(ILogger<AdminController> logger, UserService userService)
    {
        _userService = userService;
        _logger = logger;
    }

    public IActionResult Index()
    {

        return View();
    }



    public IActionResult Login()
    {
        return View();
    }



    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
