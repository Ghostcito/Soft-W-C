using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Soft_W_C.Data;
using Soft_W_C.Models;

namespace Soft_W_C.Service
{
    public class AsistenciaService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserService _userService;
        public AsistenciaService(ApplicationDbContext context, UserService userService)
        {
            _userService = userService;
            _context = context;
        }

        public async Task<Asistencia> AddEntrada()
        {
            var userPrincipal = _userService.GetCurrentUserAsync();
            var asistencia = new Asistencia
            {
                IdEmpleado = userPrincipal.Result.Id,
                Empleado = userPrincipal.Result,
                Fecha = DateTime.Now.Date,
                HoraEntrada = DateTime.Now,
                Presente = true,
            };
            AddAsistencia(asistencia);
            return asistencia;

        }

        public async Task<Asistencia?> AddSalida()
        {
            var userPrincipal = _userService.GetCurrentUserAsync();
            var asistencia = GetAllAsistenciaByIdEmpleado(userPrincipal.Result.Id).FindLast(a => a.Fecha == DateTime.Now.Date);
            if (asistencia != null && asistencia.HoraEntrada.HasValue)
            {
                asistencia.HoraSalida = DateTime.Now;
                UpdateAsistencia(asistencia);
                return asistencia;
            }
            else
            {
                Console.WriteLine("No se encontró la asistencia para el empleado o no se registró la hora de entrada.");
                return null;
            }
        }

        public void CalcularHorasTrabajadas(int id)
        {
            var asistencia = _context.Asistencia.Find(id);
            if (asistencia != null && asistencia.HoraEntrada.HasValue && asistencia.HoraSalida.HasValue)
            {
                asistencia.HorasTrabajadas = (decimal)(asistencia.HoraSalida - asistencia.HoraEntrada).Value.TotalHours;
                UpdateAsistencia(asistencia);
            }
        }
        public Asistencia AddAsistencia(Asistencia asistencia)
        {
            _context.Asistencia.Add(asistencia);
            _context.SaveChanges();
            return asistencia;
        }

        public List<Asistencia> GetAllAsistencias()
        {
            return _context.Asistencia.ToList();
        }

        public async Task<Asistencia> GetAllAsistenciaById(int id)
        {
            return await _context.Asistencia.FindAsync(id);
        }

        public List<Asistencia> GetAllAsistenciaByIdEmpleado(string id)
        {
            return _context.Asistencia.Where(a => a.IdEmpleado == id).ToList();
        }

        public void UpdateAsistencia(Asistencia asistencia)
        {
            _context.Asistencia.Update(asistencia);
            _context.SaveChanges();
        }
        public void DeleteAsistencia(int id)
        {
            var asistencia = _context.Asistencia.Find(id);
            if (asistencia != null)
            {
                _context.Asistencia.Remove(asistencia);
                _context.SaveChanges();
            }
        }



    }
}