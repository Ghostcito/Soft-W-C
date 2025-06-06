using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
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

        public async Task<Asistencia> GetAsistenciaByDate(DateTime fecha)
        {
            var userPrincipal = await _userService.GetCurrentUserAsync();
            var asistencia = await _context.Asistencia
                .FirstOrDefaultAsync(a => a.IdEmpleado == userPrincipal.Id && a.Fecha.Date == fecha.Date);
            return asistencia;
        }

        public async Task<Asistencia?> AddSalida(Asistencia asistencia)
        {
            var limaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time");
            var horaPeru = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, limaTimeZone);
            horaPeru = DateTime.SpecifyKind(horaPeru, DateTimeKind.Utc);
            asistencia.HoraSalida = horaPeru;
            asistencia.HorasTrabajadas = await CalcularHorasTrabajadas((DateTime)asistencia.HoraEntrada, (DateTime)asistencia.HoraSalida);
            return asistencia;
        }

        public async Task<decimal> CalcularHorasTrabajadas(DateTime horaEntrada, DateTime horaSalida)
        {
            return (decimal)(horaSalida - horaEntrada).TotalHours;
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

        public async Task<(Sede, bool)> ValidarDistancia(UbicacionDTO ubicacion)
        {
            var empleado = await _context.Usuario
            .Include(u => u.Sedes)
            .FirstOrDefaultAsync(u => u.Id == ubicacion.EmpleadoId);
            if (empleado.Sedes == null || !empleado.Sedes.Any())
                return (null, false);
            return await DetectarSede(Convert.ToDecimal(ubicacion.Latitud), Convert.ToDecimal(ubicacion.Longitud), empleado.Sedes);
        }

        public async Task<(Sede, bool)> DetectarSede(decimal latitud, decimal longitud, ICollection<Sede> sedes)
        {
            double distMin = double.MaxValue;
            Sede sedeCercana = sedes.FirstOrDefault();
            foreach (var sede in sedes)
            {
                var distancia = GeoUtils.CalcularDistancia(Convert.ToDouble(latitud), Convert.ToDouble(longitud), Convert.ToDouble(sede.Latitud), Convert.ToDouble(sede.Longitud));
                if (distancia < distMin)
                {
                    sedeCercana = sede;
                    distMin = distancia;
                }
            }
            if (distMin <= Convert.ToDouble(sedeCercana.Radio))
            {
                return (sedeCercana, false);
            }
            return (sedeCercana, true);
        }

        public async Task<bool> VerificarUnicaEntrada(DateTime fecha)
        {
            var userPrincipal = await _userService.GetCurrentUserAsync();
            var asistencia = await _context.Asistencia
                .FirstOrDefaultAsync(a => a.IdEmpleado == userPrincipal.Id && a.Fecha.Date == fecha.Date && a.HoraEntrada != null);
            return asistencia == null;
        }

        public async Task<bool> ValidarAsistenciaEnTurno(Asistencia asistencia)
        {
            var userPrincipal = await _userService.GetCurrentUserAsync();

            // Buscar el turno asignado para la fecha de la asistencia
            var usuarioTurno = await _context.UsuarioTurno
                .Include(ut => ut.Turno)
                .FirstOrDefaultAsync(ut =>
                    ut.UsuarioId == userPrincipal.Id &&
                    asistencia.Fecha.Date >= ut.FechaInicio &&
                    asistencia.Fecha.Date <= ut.FechaFin);

            if (usuarioTurno == null || usuarioTurno.Turno == null)
                return false; // No hay turno asignado

            var turno = usuarioTurno.Turno;

            // Definir los rangos permitidos con desviación de 10 minutos
            var margen = TimeSpan.FromMinutes(10);

            // Hora de entrada permitida
            var horaEntradaEsperada = asistencia.Fecha.Date.Add(turno.HoraInicio);
            var entradaMin = horaEntradaEsperada - margen;
            var entradaMax = horaEntradaEsperada + margen;

            // Hora de salida permitida
            var horaSalidaEsperada = asistencia.Fecha.Date.Add(turno.HoraFin);
            var salidaMin = horaSalidaEsperada - margen;
            var salidaMax = horaSalidaEsperada + margen;

            bool entradaValida = asistencia.HoraEntrada.HasValue &&
                                 asistencia.HoraEntrada.Value >= entradaMin &&
                                 asistencia.HoraEntrada.Value <= entradaMax;

            bool salidaValida = true; // Por defecto true si no se marca salida
            if (asistencia.HoraSalida.HasValue)
            {
                salidaValida = asistencia.HoraSalida.Value >= salidaMin &&
                               asistencia.HoraSalida.Value <= salidaMax;
            }

            // Debe cumplir ambos si hay entrada y salida
            return entradaValida && salidaValida;
        }


        public List<Asistencia> GetAllAsistencias()
        {
            return _context.Asistencia.ToList();
        }

        public async Task<Asistencia> GetAllAsistenciaById(int id)
        {
            return await _context.Asistencia.FindAsync(id);
        }

        public async Task<List<Asistencia>> GetAllAsistenciaByIdEmpleado(string id)
        {
            return await _context.Asistencia.Where(a => a.IdEmpleado == id).ToListAsync();
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