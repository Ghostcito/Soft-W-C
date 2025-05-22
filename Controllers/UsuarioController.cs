using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoftWC.Data;
using SoftWC.Models;
using SoftWC.Service;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace SoftWC.Controllers
{
    public class UsuarioController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;
        private readonly EmpleadoService _empleadoService;
        private readonly UserManager<Usuario> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsuarioController(ApplicationDbContext context, UserService userService, EmpleadoService empleadoService, UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager)        {
            _empleadoService = empleadoService;
            _userService = userService;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        // GET: Usuario
        public async Task<IActionResult> Index()
        {
            var usuarios = await _empleadoService.GetEmpleados();
            return View(usuarios);
        }

        public async Task<IActionResult> FindAllBySede(int id)
        {
            var usuarios = await _context.Usuario
            .Include(u => u.Sedes)
            .Where(u => u.Sedes.Any(s => s.SedeId == id) &&
                        (u.NivelAcceso == "1" || u.NivelAcceso == "2"))
            .ToListAsync();
            return View("Index", usuarios);
        }

        // GET: Usuario/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // GET: Usuario/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usuario/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Apellido,DNI,FechaIngreso,FechaNacimiento,NivelAcceso,Estado,Salario,UserName,Email,PasswordHash,PhoneNumber")] Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                var nombreUsuario = usuario.Nombre + " " + usuario.Apellido;
                usuario.UserName = nombreUsuario;
                _context.Add(usuario);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(usuario);
        }

        // GET: Usuario/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }
            ViewData["RoleDisponibles"] = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
            return View(usuario);
        }

        // POST: Usuario/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Nombre,Apellido,DNI,FechaIngreso,FechaNacimiento,NivelAcceso,Estado,Salario,Id,UserName,Email,PhoneNumber")] Usuario usuario, string PasswordActual, string NuevaContrasena, string ConfirmarContrasena, string RolSeleccionado)
        {
            Console.WriteLine("Editando usuario: " + usuario.Id);
            ViewData["RoleDisponibles"] = new SelectList(_roleManager.Roles.ToList(), "Name", "Name");
            if (id != usuario.Id)
                return NotFound();

            var usuarioActual = await _userService.FindByDniAsync(usuario.DNI);
            if (usuarioActual == null)
                return NotFound();

            ModelState.Remove("PasswordActual");
            ModelState.Remove("NuevaContrasena");
            ModelState.Remove("ConfirmarContrasena");          
            ModelState.Remove("RolSeleccionado");

            if (ModelState.IsValid)
            {
                try
                {
                    // Actualizar campos comunes
                    usuarioActual.Nombre = usuario.Nombre;
                    usuarioActual.Apellido = usuario.Apellido;
                    usuarioActual.DNI = usuario.DNI;
                    usuarioActual.FechaIngreso = usuario.FechaIngreso.HasValue
                    ? DateTime.SpecifyKind(usuario.FechaIngreso.Value, DateTimeKind.Local).ToUniversalTime()
                    : null;

                    usuarioActual.FechaNacimiento = usuario.FechaNacimiento.HasValue
                        ? DateTime.SpecifyKind(usuario.FechaNacimiento.Value, DateTimeKind.Local).ToUniversalTime()
                        : null;

                    usuarioActual.NivelAcceso = usuario.NivelAcceso;
                    usuarioActual.Estado = usuario.Estado;
                    usuarioActual.Salario = usuario.Salario;
                    usuarioActual.UserName = usuario.Nombre;
                    usuarioActual.Email = usuario.Email;
                    usuarioActual.PhoneNumber = usuario.PhoneNumber;                    

                    // Validar y cambiar la contraseña
                    if (!string.IsNullOrWhiteSpace(PasswordActual) && 
                        !string.IsNullOrWhiteSpace(NuevaContrasena) &&
                        !string.IsNullOrWhiteSpace(ConfirmarContrasena))
                    {
                        if (NuevaContrasena != ConfirmarContrasena)
                        {
                            ModelState.AddModelError("", "La nueva contraseña y la confirmación no coinciden.");
                            return View(usuario);
                        }

                        var passwordCorrecta = await _userManager.CheckPasswordAsync(usuarioActual, PasswordActual);
                        if (!passwordCorrecta)
                        {
                            ModelState.AddModelError("", "La contraseña actual no es correcta.");
                            return View(usuario);
                        }

                        var resultado = await _userManager.ChangePasswordAsync(usuarioActual, PasswordActual, NuevaContrasena);
                        if (!resultado.Succeeded)
                        {
                            if (!string.IsNullOrEmpty(RolSeleccionado))
                            {
                                await _userManager.AddToRoleAsync(usuarioActual, RolSeleccionado);
                            }

                            foreach (var error in resultado.Errors)
                            {
                                ModelState.AddModelError("", error.Description);
                            }
                            return View(usuario);
                        }
                    }

                    Console.WriteLine("Editando usuario x33: " + usuario.Id);
                    _context.Update(usuarioActual);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioExists(usuario.Id))
                        return NotFound();
                    else
                        throw;
                }
            }else{
                foreach (var key in ModelState.Keys)
                {
                    var state = ModelState[key];
                    foreach (var error in state.Errors)
                    {
                        Console.WriteLine($"Error en {key}: {error.ErrorMessage}");
                    }
                }
                return View(usuario);
            }

            return View(usuario);
        }


        // GET: Usuario/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuario
                .FirstOrDefaultAsync(m => m.Id == id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuario/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var usuario = await _context.Usuario.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuario.Remove(usuario);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioExists(string id)
        {
            return _context.Usuario.Any(e => e.Id == id);
        }
    }
}
