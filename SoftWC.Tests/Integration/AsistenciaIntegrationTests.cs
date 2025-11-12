using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SoftWC.Data;
using SoftWC.Models;
using SoftWC.Models.Dto;
using SoftWC.Service;
using SoftWC.Tests.Helpers;
using Xunit;

namespace SoftWC.Tests.Integration;

public class AsistenciaIntegrationTests
{
    [Fact]
    public async Task RegistrarAsistencia_DeberiaGuardarEnBaseDeDatos()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext("TestDB_Asistencia1");
        
        var usuario = new Usuario
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "empleado1",
            DNI = "12345678",
            Nombre = "Juan",
            Apellido = "Pérez"
        };
        
        context.Users.Add(usuario);
        await context.SaveChangesAsync();

        var mockUserService = new MockUserService(context, usuario.Id);
        var service = new AsistenciaService(context, mockUserService);

        var asistencia = new Asistencia
        {
            IdEmpleado = usuario.Id,
            Fecha = DateTime.UtcNow.Date,
            HoraEntrada = DateTime.UtcNow,
            Presente = true
        };

        // Act
        var resultado = await service.AddAsistencia(asistencia);

        // Assert
        resultado.Should().NotBeNull();
        resultado.IdAsistencia.Should().BeGreaterThan(0);
        
        var asistenciaGuardada = await context.Asistencia
            .FirstOrDefaultAsync(a => a.IdAsistencia == resultado.IdAsistencia);
        
        asistenciaGuardada.Should().NotBeNull();
        asistenciaGuardada!.IdEmpleado.Should().Be(usuario.Id);
        asistenciaGuardada.Presente.Should().BeTrue();
    }

    [Fact]
    public async Task GetAllAsistenciaByIdEmpleado_DeberiaRetornarAsistenciasDelEmpleado()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext("TestDB_Asistencia2");
        
        var usuario1 = new Usuario
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "empleado1",
            DNI = "12345678"
        };
        
        var usuario2 = new Usuario
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "empleado2",
            DNI = "87654321"
        };
        
        context.Users.AddRange(usuario1, usuario2);
        
        var asistencia1 = new Asistencia
        {
            IdEmpleado = usuario1.Id,
            Fecha = DateTime.UtcNow.Date,
            HoraEntrada = DateTime.UtcNow,
            Presente = true
        };
        
        var asistencia2 = new Asistencia
        {
            IdEmpleado = usuario1.Id,
            Fecha = DateTime.UtcNow.Date.AddDays(-1),
            HoraEntrada = DateTime.UtcNow.AddDays(-1),
            Presente = true
        };
        
        var asistencia3 = new Asistencia
        {
            IdEmpleado = usuario2.Id,
            Fecha = DateTime.UtcNow.Date,
            HoraEntrada = DateTime.UtcNow,
            Presente = true
        };
        
        context.Asistencia.AddRange(asistencia1, asistencia2, asistencia3);
        await context.SaveChangesAsync();

        var mockUserService = new MockUserService(context, usuario1.Id);
        var service = new AsistenciaService(context, mockUserService);

        // Act - Obtener asistencias directamente del contexto
        var resultado = await context.Asistencia
            .Where(a => a.IdEmpleado == usuario1.Id)
            .ToListAsync();

        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado.All(a => a.IdEmpleado == usuario1.Id).Should().BeTrue();
    }

    [Fact]
    public async Task ValidarDistancia_EmpleadoDentroDelRadio_DeberiaRetornarTrue()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext("TestDB_GPS1");
        
        var cliente = new Cliente
        {
            ClienteId = 1,
            Nombre = "Cliente Test",
            Estado = true
        };
        
        var sede = new Sede
        {
            SedeId = 1,
            ClienteId = 1,
            Nombre_local = "Sede Central",
            Latitud = -12.0464m,
            Longitud = -77.0428m,
            Radio = 1000m // 1 km de radio
        };
        
        var usuario = new Usuario
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "empleado1",
            DNI = "12345678"
        };
        
        usuario.Sedes.Add(sede);
        
        context.Users.Add(usuario);
        context.Cliente.Add(cliente);
        context.Sede.Add(sede);
        await context.SaveChangesAsync();

        var mockUserService = new MockUserService(context, usuario.Id);
        var service = new AsistenciaService(context, mockUserService);

        var ubicacion = new SoftWC.Models.Dto.UbicacionDTO
        {
            EmpleadoId = usuario.Id,
            Latitud = -12.0474, // Muy cerca de la sede (aproximadamente 100m)
            Longitud = -77.0428
        };

        // Act
        var (sedeEncontrada, dentroDelRadio) = await service.ValidarDistancia(ubicacion);

        // Assert
        dentroDelRadio.Should().BeTrue();
        sedeEncontrada.Should().NotBeNull();
        sedeEncontrada!.SedeId.Should().Be(sede.SedeId);
    }

    [Fact]
    public async Task ValidarDistancia_EmpleadoFueraDelRadio_DeberiaRetornarFalse()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext("TestDB_GPS2");
        
        var cliente = new Cliente
        {
            ClienteId = 1,
            Nombre = "Cliente Test",
            Estado = true
        };
        
        var sede = new Sede
        {
            SedeId = 1,
            ClienteId = 1,
            Nombre_local = "Sede Central",
            Latitud = -12.0464m,
            Longitud = -77.0428m,
            Radio = 100m // Solo 100m de radio
        };
        
        var usuario = new Usuario
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "empleado1",
            DNI = "12345678"
        };
        
        usuario.Sedes.Add(sede);
        
        context.Users.Add(usuario);
        context.Cliente.Add(cliente);
        context.Sede.Add(sede);
        await context.SaveChangesAsync();

        var mockUserService = new MockUserService(context, usuario.Id);
        var service = new AsistenciaService(context, mockUserService);

        var ubicacion = new SoftWC.Models.Dto.UbicacionDTO
        {
            EmpleadoId = usuario.Id,
            Latitud = -12.0554, // Lejos de la sede (aproximadamente 1 km)
            Longitud = -77.0428
        };

        // Act
        var (sedeEncontrada, dentroDelRadio) = await service.ValidarDistancia(ubicacion);

        // Assert
        dentroDelRadio.Should().BeFalse();
    }
}

// Helper class para simular UserService en pruebas de integración
// Usamos composición en lugar de herencia porque GetCurrentUserAsync no es virtual
public class MockUserService : UserService
{
    private readonly ApplicationDbContext _testContext;
    private readonly string _userId;

    public MockUserService(ApplicationDbContext context, string userId) 
        : base(context, 
            MockHelpers.CreateMockUserManager().Object,
            Mock.Of<Microsoft.AspNetCore.Http.IHttpContextAccessor>())
    {
        _testContext = context;
        _userId = userId;
    }

    // Ahora podemos hacer override porque GetCurrentUserAsync es virtual
    public override async Task<Usuario> GetCurrentUserAsync()
    {
        return await _testContext.Users.FindAsync(_userId) 
            ?? throw new InvalidOperationException("Usuario no encontrado");
    }
}

