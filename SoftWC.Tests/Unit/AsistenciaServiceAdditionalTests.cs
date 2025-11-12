using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SoftWC.Data;
using SoftWC.Models;
using SoftWC.Service;
using SoftWC.Tests.Helpers;
using Xunit;

namespace SoftWC.Tests.Unit;

public class AsistenciaServiceAdditionalTests
{
    [Fact]
    public async Task GetAsistenciaByDate_AsistenciaExistente_DeberiaRetornarAsistencia()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext("TestDB_GetByDate");
        var mockUserService = new Mock<UserService>(
            context,
            MockHelpers.CreateMockUserManager().Object,
            Mock.Of<Microsoft.AspNetCore.Http.IHttpContextAccessor>()
        );
        
        var usuario = new Usuario
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testuser",
            DNI = "12345678",
            Nombre = "Test",
            Apellido = "User"
        };
        
        context.Users.Add(usuario);
        
        var fecha = DateTime.UtcNow.Date;
        var asistencia = new Asistencia
        {
            IdEmpleado = usuario.Id,
            Empleado = usuario,
            Fecha = fecha,
            HoraEntrada = fecha.AddHours(8),
            Presente = true
        };
        
        context.Asistencia.Add(asistencia);
        await context.SaveChangesAsync();

        // Configurar mock para retornar el usuario
        mockUserService.Setup(x => x.GetCurrentUserAsync())
            .ReturnsAsync(usuario);

        var service = new AsistenciaService(context, mockUserService.Object);

        // Act
        var resultado = await service.GetAsistenciaByDate(fecha);

        // Assert
        resultado.Should().NotBeNull();
        resultado.IdEmpleado.Should().Be(usuario.Id);
        resultado.Fecha.Date.Should().Be(fecha);
    }

    [Fact]
    public async Task VerificarUnicaEntrada_SinEntradaPrevia_DeberiaRetornarTrue()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext("TestDB_UnicaEntrada");
        var mockUserService = new Mock<UserService>(
            context,
            MockHelpers.CreateMockUserManager().Object,
            Mock.Of<Microsoft.AspNetCore.Http.IHttpContextAccessor>()
        );
        
        var usuario = new Usuario
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testuser",
            DNI = "12345678"
        };
        
        context.Users.Add(usuario);
        await context.SaveChangesAsync();

        mockUserService.Setup(x => x.GetCurrentUserAsync())
            .ReturnsAsync(usuario);

        var service = new AsistenciaService(context, mockUserService.Object);
        var fecha = DateTime.UtcNow.Date;

        // Act
        var resultado = await service.VerificarUnicaEntrada(fecha);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task VerificarUnicaEntrada_ConEntradaPrevia_DeberiaRetornarFalse()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext("TestDB_UnicaEntrada2");
        var mockUserService = new Mock<UserService>(
            context,
            MockHelpers.CreateMockUserManager().Object,
            Mock.Of<Microsoft.AspNetCore.Http.IHttpContextAccessor>()
        );
        
        var usuario = new Usuario
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testuser",
            DNI = "12345678"
        };
        
        context.Users.Add(usuario);
        
        var fecha = DateTime.UtcNow.Date;
        var asistencia = new Asistencia
        {
            IdEmpleado = usuario.Id,
            Empleado = usuario,
            Fecha = fecha,
            HoraEntrada = fecha.AddHours(8),
            Presente = true
        };
        
        context.Asistencia.Add(asistencia);
        await context.SaveChangesAsync();

        mockUserService.Setup(x => x.GetCurrentUserAsync())
            .ReturnsAsync(usuario);

        var service = new AsistenciaService(context, mockUserService.Object);

        // Act
        var resultado = await service.VerificarUnicaEntrada(fecha);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task DetectarSede_EmpleadoCercaDeSede_DeberiaRetornarTrue()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext("TestDB_DetectarSede");
        var mockUserService = new Mock<UserService>(
            context,
            MockHelpers.CreateMockUserManager().Object,
            Mock.Of<Microsoft.AspNetCore.Http.IHttpContextAccessor>()
        );
        
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
            Cliente = cliente,
            Nombre_local = "Sede Test",
            Latitud = -12.0464m,
            Longitud = -77.0428m,
            Radio = 100m // 100 metros
        };
        
        context.Cliente.Add(cliente);
        context.Sede.Add(sede);
        await context.SaveChangesAsync();

        var sedes = new List<Sede> { sede };
        
        var service = new AsistenciaService(context, mockUserService.Object);

        // Act - Coordenadas muy cerca de la sede (dentro del radio)
        var (sedeDetectada, dentroDelRadio) = await service.DetectarSede(
            -12.0464m, // Misma latitud
            -77.0428m, // Misma longitud
            sedes
        );

        // Assert
        dentroDelRadio.Should().BeTrue();
        sedeDetectada.Should().NotBeNull();
        sedeDetectada.SedeId.Should().Be(1);
    }
}

