using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoftWC.Data;
using SoftWC.Models;
using Microsoft.AspNetCore.Identity;

namespace SoftWC.Controllers
{
    public class SupervisionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupervisionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Supervision
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Supervision.Include(s => s.Empleado).Include(s => s.Supervisor);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Supervision/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervision = await _context.Supervision
                .Include(s => s.Empleado)
                .Include(s => s.Supervisor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (supervision == null)
            {
                return NotFound();
            }

            return View(supervision);
        }

        // GET: Supervision/Create
        public async Task<IActionResult> Create()
        {   
            // Obtener el ID del rol "Empleado"
            var empleadoRole = await _context.Roles
                .Where(r => r.Name == "Empleado")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            // Obtener los IDs de los usuarios que tienen el rol "Administrador"
            var adminUserIds = await _context.UserRoles
                .Where(ur => ur.RoleId == empleadoRole)
                .Select(ur => ur.UserId)
                .ToListAsync();

            // Crear SelectList con Nombre + Apellido solo para administradores
            ViewData["EmpleadoId"] = new SelectList(
                _context.Users
                    .Where(u => adminUserIds.Contains(u.Id))
                    .Select(u => new
                    {
                        u.Id,
                        NombreCompleto = u.Nombre + " " + u.Apellido
                    }),
                "Id", "NombreCompleto");

            // Obtener el ID del rol "Empleado"
            var supervisorRole = await _context.Roles
                .Where(r => r.Name == "Supervisor")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            // Obtener los IDs de los usuarios que tienen el rol "Administrador"
            var supervisorUserIds = await _context.UserRoles
                .Where(ur => ur.RoleId == supervisorRole)
                .Select(ur => ur.UserId)
                .ToListAsync();


            // Supervisores: todos los usuarios
            ViewData["SupervisorId"] = new SelectList(
                _context.Users
                    .Where(u => supervisorUserIds.Contains(u.Id))
                    .Select(u => new
                    {
                        u.Id,
                        NombreCompleto = u.Nombre + " " + u.Apellido
                    }),
                "Id", "NombreCompleto");

            return View();
        }

        // POST: Supervision/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SupervisorId,EmpleadoId,FechaInicio,FechaFin")] Supervision supervision)
        {
            if (ModelState.IsValid)
            {
                _context.Add(supervision);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmpleadoId"] = new SelectList(_context.Usuario, "Id", "Nombre", supervision.EmpleadoId);
            ViewData["SupervisorId"] = new SelectList(_context.Usuario, "Id", "Nombre", supervision.SupervisorId);
            return View(supervision);
        }

        // GET: Supervision/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervision = await _context.Supervision.FindAsync(id);
            if (supervision == null)
            {
                return NotFound();
            }
            ViewData["EmpleadoId"] = new SelectList(_context.Usuario, "Id", "Id", supervision.EmpleadoId);
            ViewData["SupervisorId"] = new SelectList(_context.Usuario, "Id", "Id", supervision.SupervisorId);
            return View(supervision);
        }

        // POST: Supervision/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SupervisorId,EmpleadoId,FechaInicio,FechaFin")] Supervision supervision)
        {
            if (id != supervision.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supervision);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupervisionExists(supervision.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmpleadoId"] = new SelectList(_context.Usuario, "Id", "Id", supervision.EmpleadoId);
            ViewData["SupervisorId"] = new SelectList(_context.Usuario, "Id", "Id", supervision.SupervisorId);
            return View(supervision);
        }

        // GET: Supervision/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supervision = await _context.Supervision
                .Include(s => s.Empleado)
                .Include(s => s.Supervisor)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (supervision == null)
            {
                return NotFound();
            }

            return View(supervision);
        }

        // POST: Supervision/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supervision = await _context.Supervision.FindAsync(id);
            if (supervision != null)
            {
                _context.Supervision.Remove(supervision);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SupervisionExists(int id)
        {
            return _context.Supervision.Any(e => e.Id == id);
        }
    }
}
