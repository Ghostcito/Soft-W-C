using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using SoftWC.Data;
using SoftWC.Models;

namespace SoftWC.Service
{
    public class TareoService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;
        private readonly AsistenciaService _asistenciaService;
        public TareoService(ApplicationDbContext context, UserService userService, AsistenciaService asistenciaService)
        {
            _userService = userService;
            _asistenciaService = asistenciaService;
            _context = context;
        }

        // Método para agregar un tareo
        public async Task<Tareo> AddTareo(Tareo tareo)
        {
            var userPrincipal = await _userService.GetCurrentUserAsync();
            tareo.IdEmpleado = userPrincipal.Id;
            tareo.Fecha = DateTime.SpecifyKind(tareo.Fecha, DateTimeKind.Utc);
            _context.Tareo.Add(tareo);
            await _context.SaveChangesAsync();
            return tareo;
        }

        // Método para obtener todos los tareos
        public async Task<List<Tareo>> GetAllTareos()
        {
            return await _context.Tareo
                .Include(t => t.Empleado)
                .Include(t => t.Servicio)
                .Include(t => t.Turno)
                .ToListAsync();
        }
        // Método para obtener un tareo por ID
        public async Task<Tareo?> GetTareoById(int id)
        {
            return await _context.Tareo
                .Include(t => t.Empleado)
                .Include(t => t.Servicio)
                .Include(t => t.Turno)
                .FirstOrDefaultAsync(t => t.IdTareo == id);
        }
        // Método para actualizar un tareo
        public async Task<Tareo?> UpdateTareo(Tareo tareo)
        {
            var existingTareo = await GetTareoById(tareo.IdTareo);
            if (existingTareo == null)
            {
                return null; // Tareo no encontrado
            }

            existingTareo.Fecha = tareo.Fecha;
            existingTareo.ServicioId = tareo.ServicioId;
            existingTareo.HorasTrabajadas = tareo.HorasTrabajadas;
            existingTareo.PagoPorHora = tareo.PagoPorHora;
            existingTareo.TotalGanado = tareo.TotalGanado;
            existingTareo.Observacion = tareo.Observacion;
            existingTareo.TurnoId = tareo.TurnoId;
            existingTareo.Estado = tareo.Estado;

            _context.Tareo.Update(existingTareo);
            await _context.SaveChangesAsync();
            return existingTareo;
        }

        // Método para eliminar un tareo
        public async Task<bool> DeleteTareo(int id)
        {
            var tareo = await GetTareoById(id);
            if (tareo == null)
            {
                return false; // Tareo no encontrado
            }

            _context.Tareo.Remove(tareo);
            await _context.SaveChangesAsync();
            return true; // Eliminación exitosa
        }

        // Método para obtener tareos por empleado
        public async Task<List<Tareo>> GetTareosByEmpleado(string empleadoId)
        {
            return await _context.Tareo
                .Where(t => t.IdEmpleado == empleadoId)
                .Include(t => t.Empleado)
                .Include(t => t.Servicio)
                .Include(t => t.Turno)
                .ToListAsync();
        }

        public async Task<(bool, decimal)> CalcularHorasTotales(DateTime fechaInicio, DateTime fechFin)
        {
            var asistencias = await _asistenciaService.GetAsistenciaByDateRange(DateTime.SpecifyKind(fechaInicio, DateTimeKind.Utc), DateTime.SpecifyKind(fechFin, DateTimeKind.Utc));

            if (asistencias == null || asistencias.Count() < 15)
            {
                return (false, 0);
            }
            var totalHoras = asistencias.Sum(a => a.HorasTrabajadas);

            return (true, totalHoras);


        }
    }
}