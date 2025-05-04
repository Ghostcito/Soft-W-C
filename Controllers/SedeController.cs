using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Soft_W_C.Data;
using Soft_W_C.Models;

namespace Soft_W_C.Controllers
{
    public class SedeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SedeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Sede By Cliente Id
        public async Task<IActionResult> Index(int id)
        {
            var applicationDbContext = _context.Sede.Include(s => s.Cliente).Where(s => s.ClienteId == id);
            return View(await applicationDbContext.ToListAsync());
        }

        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Sede.Include(s => s.Cliente);
            return View(await applicationDbContext.ToListAsync());
        }


        // GET: Sede/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sede = await _context.Sede
                .Include(s => s.Cliente)
                .FirstOrDefaultAsync(m => m.SedeId == id);
            if (sede == null)
            {
                return NotFound();
            }

            return View(sede);
        }

        // GET: Sede/Create
        public IActionResult Create()
        {
            ViewData["ClienteId"] = new SelectList(_context.Cliente, "ClienteId", "ClienteId");
            return View();
        }

        // POST: Sede/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SedeId,ClienteId,Nombre_local,Direccion_local,Ciudad,Provincia,Latitud,Longitud,estadoSede")] Sede sede)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sede);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClienteId"] = new SelectList(_context.Cliente, "ClienteId", "ClienteId", sede.ClienteId);
            return View(sede);
        }

        // GET: Sede/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sede = await _context.Sede.FindAsync(id);
            if (sede == null)
            {
                return NotFound();
            }
            ViewData["ClienteId"] = new SelectList(_context.Cliente, "ClienteId", "ClienteId", sede.ClienteId);
            return View(sede);
        }

        // POST: Sede/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SedeId,ClienteId,Nombre_local,Direccion_local,Ciudad,Provincia,Latitud,Longitud,estadoSede")] Sede sede)
        {
            if (id != sede.SedeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sede);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SedeExists(sede.SedeId))
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
            ViewData["ClienteId"] = new SelectList(_context.Cliente, "ClienteId", "ClienteId", sede.ClienteId);
            return View(sede);
        }

        // GET: Sede/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sede = await _context.Sede
                .Include(s => s.Cliente)
                .FirstOrDefaultAsync(m => m.SedeId == id);
            if (sede == null)
            {
                return NotFound();
            }

            return View(sede);
        }

        // POST: Sede/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var sede = await _context.Sede.FindAsync(id);
            if (sede != null)
            {
                _context.Sede.Remove(sede);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SedeExists(int id)
        {
            return _context.Sede.Any(e => e.SedeId == id);
        }
    }
}
