using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prueba_Geolocalizacion.Utils;
using SoftWC.Data;
using SoftWC.Models;
using SoftWC.Models.Dto;

namespace SoftWC.Service
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
            var userPrincipal = await _userService.GetCurrentUserAsync();

            //CAMBIANDO ZONA HORARIO
            var limaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            var horaPeru = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, limaTimeZone);
            horaPeru = DateTime.SpecifyKind(horaPeru, DateTimeKind.Utc);

            var asistencia = new Asistencia
            {
                IdEmpleado = userPrincipal.Id,
                Empleado = userPrincipal,
                Fecha = horaPeru.Date,
                HoraEntrada = horaPeru,
                Presente = true,
            };
            return asistencia;

        }

        public async Task<Asistencia?> AddSalida()
        {
            var userPrincipal = _userService.GetCurrentUserAsync();
            var asistencia = GetAllAsistenciaByIdEmpleado(userPrincipal.Result.Id).FindLast(a => a.Fecha == DateTime.UtcNow.Date);
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
        public async Task<Asistencia> AddAsistencia(Asistencia asistencia)
        {
            _context.Asistencia.Add(asistencia);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error al guardar asistencia: " + ex.InnerException?.Message, ex);
            }
            return asistencia;
        }

        public async Task<(Sede?, bool)> ValidarDistancia(UbicacionDTO ubicacion)
        {
            var empleado = await _userService.GetCurrentUserAsync();
            return DetectarSede(Convert.ToDecimal(ubicacion.Latitud), Convert.ToDecimal(ubicacion.Longitud), empleado.Sedes).Result;
        }

        public async Task<(Sede?, bool)> DetectarSede(decimal latitud, decimal longitud, ICollection<Sede> sedes)
        {
            double distMin = double.MaxValue;
            Sede sedeCercana = sedes.FirstOrDefault();
            foreach (var sede in sedes)
            {
                Console.WriteLine($"Sede: {sede.Nombre_local}, Latitud: {sede.Latitud}, Longitud: {sede.Longitud}");
                var distancia = GeoUtils.CalcularDistancia(Convert.ToDouble(latitud), Convert.ToDouble(longitud), Convert.ToDouble(sede.Latitud), Convert.ToDouble(sede.Longitud));
                if (distancia < distMin)
                {

                    sedeCercana = sede;
                    distMin = distancia;
                }
            }
            if (distMin <= Convert.ToDouble(sedeCercana.Radio))
            {
                return (sedeCercana, true);
            }
            return (sedeCercana, false);
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