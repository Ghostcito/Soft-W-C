using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoftWC.Data;
using SoftWC.Models;

namespace SoftWC.Controllers
{
    [Authorize(Roles = "Administrador,Supervisor")]
    public class UsuarioTurnoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsuarioTurnoController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: UsuarioTurno
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.UsuarioTurno.Include(u => u.Turno).Include(u => u.Usuario);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: UsuarioTurno/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarioTurno = await _context.UsuarioTurno
                .Include(u => u.Turno)
                .Include(u => u.Usuario)
                .FirstOrDefaultAsync(m => m.UsuarioTurnoId == id);
            if (usuarioTurno == null)
            {
                return NotFound();
            }

            return View(usuarioTurno);
        }

        // GET: UsuarioTurno/Create
        public IActionResult Create()
        {
            ViewData["TurnoId"] = new SelectList(_context.Turno, "TurnoId", "NombreTurno");
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "Id", "UserName");
            return View();
        }

        // POST: UsuarioTurno/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UsuarioTurnoId,UsuarioId,TurnoId,FechaInicio,FechaFin,Activo")] UsuarioTurno usuarioTurno)
        {
            usuarioTurno.Usuario = await _context.Usuario.FindAsync(usuarioTurno.UsuarioId);
            usuarioTurno.Turno = await _context.Turno.FindAsync(usuarioTurno.TurnoId);
            if (ModelState.IsValid)
            {
                _context.Add(usuarioTurno);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                Console.WriteLine(usuarioTurno.UsuarioId + " " + usuarioTurno.TurnoId);
                foreach (var modelError in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(modelError.ErrorMessage);
                }
                ModelState.AddModelError("", "Error al crear el Usuario Turno. Verifique los datos ingresados.");
            }
            ViewData["TurnoId"] = new SelectList(_context.Turno, "TurnoId", "NombreTurno", usuarioTurno.TurnoId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "Id", "UserName", usuarioTurno.UsuarioId);
            return View(usuarioTurno);
        }

        // GET: UsuarioTurno/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarioTurno = await _context.UsuarioTurno.FindAsync(id);
            if (usuarioTurno == null)
            {
                return NotFound();
            }
            ViewData["TurnoId"] = new SelectList(_context.Turno, "TurnoId", "NombreTurno", usuarioTurno.TurnoId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "Id", "Id", usuarioTurno.UsuarioId);
            return View(usuarioTurno);
        }

        // POST: UsuarioTurno/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UsuarioTurnoId,UsuarioId,TurnoId,FechaInicio,FechaFin,Activo")] UsuarioTurno usuarioTurno)
        {
            if (id != usuarioTurno.UsuarioTurnoId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(usuarioTurno);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UsuarioTurnoExists(usuarioTurno.UsuarioTurnoId))
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
            ViewData["TurnoId"] = new SelectList(_context.Turno, "TurnoId", "NombreTurno", usuarioTurno.TurnoId);
            ViewData["UsuarioId"] = new SelectList(_context.Usuario, "Id", "Id", usuarioTurno.UsuarioId);
            return View(usuarioTurno);
        }

        // GET: UsuarioTurno/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var usuarioTurno = await _context.UsuarioTurno
                .Include(u => u.Turno)
                .Include(u => u.Usuario)
                .FirstOrDefaultAsync(m => m.UsuarioTurnoId == id);
            if (usuarioTurno == null)
            {
                return NotFound();
            }

            return View(usuarioTurno);
        }

        // POST: UsuarioTurno/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var usuarioTurno = await _context.UsuarioTurno.FindAsync(id);
            if (usuarioTurno != null)
            {
                _context.UsuarioTurno.Remove(usuarioTurno);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UsuarioTurnoExists(int id)
        {
            return _context.UsuarioTurno.Any(e => e.UsuarioTurnoId == id);
        }
    }
}
