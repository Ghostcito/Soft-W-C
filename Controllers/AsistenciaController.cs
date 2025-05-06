using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Soft_W_C.Data;
using Soft_W_C.Models;
using Soft_W_C.Service;

namespace Soft_W_C.Controllers
{
    public class AsistenciaController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly AsistenciaService _asistenciaService;

        public AsistenciaController(ApplicationDbContext context, AsistenciaService asistenciaService)
        {
            _asistenciaService = asistenciaService;
            _context = context;
        }

        // GET: Asitencia
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Asistencia.Include(a => a.Empleado);
            return View(await applicationDbContext.ToListAsync());
        }

        [HttpGet("Index/{id}")]
        public async Task<IActionResult> Index(string id)
        {
            var asistencias = _asistenciaService.GetAllAsistenciaByIdEmpleado(id);
            return View(asistencias);
        }

        // GET: Asitencia/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asistencia = await _context.Asistencia
                .Include(a => a.Empleado)
                .FirstOrDefaultAsync(m => m.IdAsistencia == id);
            if (asistencia == null)
            {
                return NotFound();
            }

            return View(asistencia);
        }

        // GET: Asitencia/Create
        public IActionResult Create()
        {
            ViewData["IdEmpleado"] = new SelectList(_context.Usuario, "Id", "Id");
            return View();
        }

        // POST: Asitencia/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("IdAsistencia,IdEmpleado,Fecha,HoraEntrada,HoraSalida,HorasTrabajadas,Presente,Observacion")] Asistencia asistencia)
        {
            if (ModelState.IsValid)
            {
                _context.Add(asistencia);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Usuario, "Id", "Id", asistencia.IdEmpleado);
            return View(asistencia);
        }

        // GET: Asitencia/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asistencia = await _context.Asistencia.FindAsync(id);
            if (asistencia == null)
            {
                return NotFound();
            }
            ViewData["IdEmpleado"] = new SelectList(_context.Usuario, "Id", "Id", asistencia.IdEmpleado);
            return View(asistencia);
        }

        // POST: Asitencia/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("IdAsistencia,IdEmpleado,Fecha,HoraEntrada,HoraSalida,HorasTrabajadas,Presente,Observacion")] Asistencia asistencia)
        {
            if (id != asistencia.IdAsistencia)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(asistencia);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AsistenciaExists(asistencia.IdAsistencia))
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
            ViewData["IdEmpleado"] = new SelectList(_context.Usuario, "Id", "Id", asistencia.IdEmpleado);
            return View(asistencia);
        }

        // GET: Asitencia/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var asistencia = await _context.Asistencia
                .Include(a => a.Empleado)
                .FirstOrDefaultAsync(m => m.IdAsistencia == id);
            if (asistencia == null)
            {
                return NotFound();
            }

            return View(asistencia);
        }

        // POST: Asitencia/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var asistencia = await _context.Asistencia.FindAsync(id);
            if (asistencia != null)
            {
                _context.Asistencia.Remove(asistencia);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AsistenciaExists(int id)
        {
            return _context.Asistencia.Any(e => e.IdAsistencia == id);
        }
    }
}
