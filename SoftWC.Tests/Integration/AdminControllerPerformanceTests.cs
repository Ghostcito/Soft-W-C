using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SoftWC.Data;
using SoftWC.Models;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using Xunit;

namespace SoftWC.Tests.Integration;

public class AdminControllerPerformanceTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public AdminControllerPerformanceTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task SetupTestDataAsync()
    {
        var scope = _factory.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        // Solo asegurar que la BD esté creada (no borrar, ya es una BD nueva en memoria)
        await context.Database.EnsureCreatedAsync();

        // Crear cliente
        var cliente = new Cliente
        {
            ClienteId = 1,
            Nombre = "Cliente Test Performance",
            Estado = true
        };
        context.Cliente.Add(cliente);

        // Crear sedes
        var sedes = new List<Sede>();
        for (int i = 1; i <= 5; i++)
        {
            sedes.Add(new Sede
            {
                SedeId = i,
                ClienteId = 1,
                Nombre_local = $"Sede {i}",
                Latitud = -12.0464m + (i * 0.01m),
                Longitud = -77.0428m + (i * 0.01m),
                Radio = 1000m
            });
        }
        context.Sede.AddRange(sedes);

        // Crear servicio
        var servicio = new Servicio
        {
            ServicioId = 1,
            NombreServicio = "Limpieza",
            TipoServicio = "Limpieza General",
            Descripcion = "Servicio de limpieza",
            PrecioBase = 15.50m
        };
        context.Servicio.Add(servicio);

        // Crear turnos
        var turnos = new List<Turno>();
        for (int i = 1; i <= 3; i++)
        {
            turnos.Add(new Turno
            {
                TurnoId = i,
                NombreTurno = $"Turno {i}",
                HoraInicio = TimeSpan.FromHours(8 + i),
                HoraFin = TimeSpan.FromHours(16 + i),
                Descripcion = $"Descripción del turno {i}"
            });
        }
        context.Turno.AddRange(turnos);

        // Crear usuarios/empleados
        var usuarios = new List<Usuario>();
        
        for (int i = 1; i <= 20; i++)
        {
            var usuario = new Usuario
            {
                Id = $"user{i}",
                UserName = $"empleado{i}",
                Nombre = $"Empleado{i}",
                Apellido = $"Test{i}",
                DNI = $"1234567{i:D2}",
                Email = $"empleado{i}@test.com",
                FechaNacimiento = DateTime.Now.AddYears(-25 - (i % 10)),
                ServicioId = 1
            };
            usuarios.Add(usuario);
            context.Users.Add(usuario);

            // Asignar sedes a usuarios
            usuario.Sedes = sedes.Take(2).ToList();

            // Asignar turnos
            var turnoIndex = i % 3;
            context.UsuarioTurno.Add(new UsuarioTurno
            {
                UsuarioId = usuario.Id,
                TurnoId = turnos[turnoIndex].TurnoId,
                Activo = true
            });

            // Crear rol de empleado
            context.UserRoles.Add(new IdentityUserRole<string>
            {
                UserId = usuario.Id,
                RoleId = "employee_role"
            });
        }

        // Crear supervisor
        var supervisor = new Usuario
        {
            Id = "super1",
            UserName = "supervisor1",
            Nombre = "Supervisor",
            Apellido = "Test",
            DNI = "99999999",
            Email = "supervisor@test.com"
        };
        context.Users.Add(supervisor);
        context.UserRoles.Add(new IdentityUserRole<string>
        {
            UserId = supervisor.Id,
            RoleId = "supervisor_role"
        });

        // Crear relaciones de supervisión
        for (int i = 1; i <= 10; i++)
        {
            context.Supervision.Add(new Supervision
            {
                SupervisorId = supervisor.Id,
                EmpleadoId = $"user{i}"
            });
        }

        // Crear asistencias (últimos 3 meses)
        var asistencias = new List<Asistencia>();
        var fechaInicio = DateTime.UtcNow.AddMonths(-3);
        var fechaFin = DateTime.UtcNow;
        int asistenciaIdCounter = 1;

        for (int i = 0; i < usuarios.Count; i++)
        {
            var usuario = usuarios[i];
            var fechaActual = fechaInicio;

            while (fechaActual <= fechaFin)
            {
                // Crear asistencia solo en días laborables (lunes a viernes)
                if (fechaActual.DayOfWeek != DayOfWeek.Saturday && fechaActual.DayOfWeek != DayOfWeek.Sunday)
                {
                    var horaEntrada = new DateTime(fechaActual.Year, fechaActual.Month, fechaActual.Day, 8, 0, 0, DateTimeKind.Utc);
                    var horaSalida = horaEntrada.AddHours(8 + (i % 3)); // 8-10 horas trabajadas

                    asistencias.Add(new Asistencia
                    {
                        IdAsistencia = asistenciaIdCounter++,
                        IdEmpleado = usuario.Id,
                        Fecha = fechaActual,
                        HoraEntrada = horaEntrada,
                        HoraSalida = horaSalida,
                        HorasTrabajadas = 8 + (i % 3),
                        Presente = true
                    });
                }

                fechaActual = fechaActual.AddDays(1);
            }
        }
        context.Asistencia.AddRange(asistencias);

        // Crear evaluaciones
        var evaluaciones = new List<Evaluaciones>();
        foreach (var usuario in usuarios.Take(15))
        {
            evaluaciones.Add(new Evaluaciones
            {
                IdEvaluacion = evaluaciones.Count + 1,
                IdEmpleado = usuario.Id,
                TipoEmpleado = "Limpieza",
                FechaEvaluacion = DateTime.UtcNow.AddDays(-evaluaciones.Count),
                Responsabilidad = (Calificacion)((evaluaciones.Count % 4) + 1),
                Puntualidad = (Calificacion)(((evaluaciones.Count + 1) % 4) + 1),
                CalidadTrabajo = (Calificacion)(((evaluaciones.Count + 2) % 4) + 1),
                UsoMateriales = (Calificacion)(((evaluaciones.Count + 3) % 4) + 1),
                Actitud = (Calificacion)(((evaluaciones.Count + 4) % 4) + 1),
                EvaluadorId = supervisor.Id
            });
        }
        context.Evaluaciones.AddRange(evaluaciones);

        // Crear roles
        if (!context.Roles.Any(r => r.Name == "Empleado"))
        {
            context.Roles.Add(new IdentityRole { Id = "employee_role", Name = "Empleado" });
        }
        if (!context.Roles.Any(r => r.Name == "Supervisor"))
        {
            context.Roles.Add(new IdentityRole { Id = "supervisor_role", Name = "Supervisor" });
        }
        if (!context.Roles.Any(r => r.Name == "Administrador"))
        {
            context.Roles.Add(new IdentityRole { Id = "admin_role", Name = "Administrador" });
        }

        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task Index_DeberiaEjecutarseEnMenosDe2Segundos()
    {
        // Arrange
        await SetupTestDataAsync();
        var stopwatch = Stopwatch.StartNew();

        // Act
        var response = await _client.GetAsync("/Admin");

               // Assert
        stopwatch.Stop();
        var elapsedMs = stopwatch.ElapsedMilliseconds;

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Redirect, HttpStatusCode.Unauthorized);
        
        // El endpoint debería ejecutarse en menos de 2 segundos
        elapsedMs.Should().BeLessThan(2000, 
            $"El endpoint Index tardó {elapsedMs}ms, debería ser menor a 2000ms");
        
        // Log para documentación
        Console.WriteLine($"✅ Index ejecutado en {elapsedMs}ms");
    }

    [Fact]
    public async Task ResumenPagos_ConDatosCompletos_DeberiaEjecutarseEnMenosDe3Segundos()
    {
        // Arrange
        await SetupTestDataAsync();
        var año = DateTime.Now.Year;
        var mes = DateTime.Now.Month;
        var quincena = DateTime.Now.Day <= 15 ? 1 : 2;
        var stopwatch = Stopwatch.StartNew();

        // Act
        var response = await _client.GetAsync($"/Admin/ResumenPagos?año={año}&mes={mes}&quincena={quincena}");

        // Assert
        stopwatch.Stop();
        var elapsedMs = stopwatch.ElapsedMilliseconds;

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Redirect, HttpStatusCode.Unauthorized);
        
        // El endpoint debería ejecutarse en menos de 3 segundos
        elapsedMs.Should().BeLessThan(3000, 
            $"El endpoint ResumenPagos tardó {elapsedMs}ms, debería ser menor a 3000ms");
        
        Console.WriteLine($"✅ ResumenPagos ejecutado en {elapsedMs}ms");
    }

    [Fact]
    public async Task ResumenPagos_ConParametrosPorDefecto_DeberiaEjecutarseRapido()
    {
        // Arrange
        await SetupTestDataAsync();
        var stopwatch = Stopwatch.StartNew();

        // Act - Sin parámetros, debería usar valores por defecto
        var response = await _client.GetAsync("/Admin/ResumenPagos");

        // Assert
        stopwatch.Stop();
        var elapsedMs = stopwatch.ElapsedMilliseconds;

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Redirect, HttpStatusCode.Unauthorized);
        elapsedMs.Should().BeLessThan(3000, 
            $"El endpoint ResumenPagos (sin parámetros) tardó {elapsedMs}ms");
        
        Console.WriteLine($"✅ ResumenPagos (sin parámetros) ejecutado en {elapsedMs}ms");
    }

    [Fact]
    public async Task Exportar_Excel_DeberiaGenerarArchivoEnMenosDe5Segundos()
    {
        // Arrange
        await SetupTestDataAsync();
        var año = DateTime.Now.Year;
        var mes = DateTime.Now.Month;
        var quincena = DateTime.Now.Day <= 15 ? 1 : 2;
        var stopwatch = Stopwatch.StartNew();

        // Act
        var response = await _client.GetAsync($"/Admin/exportar?año={año}&mes={mes}&quincena={quincena}&formato=excel");

        // Assert
        stopwatch.Stop();
        var elapsedMs = stopwatch.ElapsedMilliseconds;

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized);
        
        // La exportación debería completarse en menos de 5 segundos
        elapsedMs.Should().BeLessThan(5000, 
            $"La exportación a Excel tardó {elapsedMs}ms, debería ser menor a 5000ms");
        
        Console.WriteLine($"✅ Exportar Excel ejecutado en {elapsedMs}ms");
    }

    [Fact]
    public async Task Exportar_PDF_DeberiaGenerarArchivoEnMenosDe5Segundos()
    {
        // Arrange
        await SetupTestDataAsync();
        var año = DateTime.Now.Year;
        var mes = DateTime.Now.Month;
        var quincena = DateTime.Now.Day <= 15 ? 1 : 2;
        var stopwatch = Stopwatch.StartNew();

        // Act
        var response = await _client.GetAsync($"/Admin/exportar?año={año}&mes={mes}&quincena={quincena}&formato=pdf");

        // Assert
        stopwatch.Stop();
        var elapsedMs = stopwatch.ElapsedMilliseconds;

        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.BadRequest, HttpStatusCode.Unauthorized);
        
        // La exportación debería completarse en menos de 5 segundos
        elapsedMs.Should().BeLessThan(5000, 
            $"La exportación a PDF tardó {elapsedMs}ms, debería ser menor a 5000ms");
        
        Console.WriteLine($"✅ Exportar PDF ejecutado en {elapsedMs}ms");
    }

    [Fact]
    public async Task Index_ConMultiplesConsultas_DeberiaMantenerRendimiento()
    {
        // Arrange
        await SetupTestDataAsync();
        var tiempos = new List<long>();
        var stopwatch = new Stopwatch();

        // Act - Ejecutar 5 veces para medir consistencia
        for (int i = 0; i < 5; i++)
        {
            stopwatch.Restart();
            var response = await _client.GetAsync("/Admin");
            stopwatch.Stop();
            
            tiempos.Add(stopwatch.ElapsedMilliseconds);
            
            // Pequeña pausa entre requests
            await Task.Delay(100);
        }

        // Assert
        var promedio = tiempos.Average();
        var maximo = tiempos.Max();
        var minimo = tiempos.Min();

        promedio.Should().BeLessThan(2000, 
            $"El tiempo promedio de Index fue {promedio}ms");
        maximo.Should().BeLessThan(3000, 
            $"El tiempo máximo de Index fue {maximo}ms");

        Console.WriteLine($"✅ Index - Promedio: {promedio:F2}ms, Mín: {minimo}ms, Máx: {maximo}ms");
    }

    [Fact]
    public async Task ResumenPagos_ConDiferentesQuincenas_DeberiaMantenerRendimiento()
    {
        // Arrange
        await SetupTestDataAsync();
        var año = DateTime.Now.Year;
        var mes = DateTime.Now.Month;
        var tiempos = new List<long>();
        var stopwatch = new Stopwatch();

        // Act - Probar ambas quincenas
        for (int quincena = 1; quincena <= 2; quincena++)
        {
            stopwatch.Restart();
            var response = await _client.GetAsync($"/Admin/ResumenPagos?año={año}&mes={mes}&quincena={quincena}");
            stopwatch.Stop();
            
            tiempos.Add(stopwatch.ElapsedMilliseconds);
            await Task.Delay(100);
        }

        // Assert
        var promedio = tiempos.Average();
        var maximo = tiempos.Max();

        promedio.Should().BeLessThan(3000, 
            $"El tiempo promedio de ResumenPagos fue {promedio}ms");
        maximo.Should().BeLessThan(4000, 
            $"El tiempo máximo de ResumenPagos fue {maximo}ms");

        Console.WriteLine($"✅ ResumenPagos - Promedio: {promedio:F2}ms, Máx: {maximo}ms");
    }
}

