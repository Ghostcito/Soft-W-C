using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using SoftWC.Data;
using SoftWC.Models;
using SoftWC.Service;
using SoftWC.Tests.Helpers;
using Xunit;

namespace SoftWC.Tests.Unit;

public class AsistenciaServiceTests
{
    [Fact]
    public async Task CalcularHorasTrabajadas_DeberiaCalcularCorrectamente()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var mockUserService = new Mock<UserService>(
            context,
            MockHelpers.CreateMockUserManager().Object,
            Mock.Of<Microsoft.AspNetCore.Http.IHttpContextAccessor>()
        );
        
        var service = new AsistenciaService(context, mockUserService.Object);
        var horaEntrada = new DateTime(2024, 1, 1, 8, 0, 0);
        var horaSalida = new DateTime(2024, 1, 1, 17, 0, 0);

        // Act
        var horas = await service.CalcularHorasTrabajadas(horaEntrada, horaSalida);

        // Assert
        horas.Should().Be(9.0m);
    }

    [Fact]
    public async Task CalcularHorasTrabajadas_MediaJornada_DeberiaCalcularCorrectamente()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var mockUserService = new Mock<UserService>(
            context,
            MockHelpers.CreateMockUserManager().Object,
            Mock.Of<Microsoft.AspNetCore.Http.IHttpContextAccessor>()
        );
        
        var service = new AsistenciaService(context, mockUserService.Object);
        var horaEntrada = new DateTime(2024, 1, 1, 8, 0, 0);
        var horaSalida = new DateTime(2024, 1, 1, 12, 30, 0);

        // Act
        var horas = await service.CalcularHorasTrabajadas(horaEntrada, horaSalida);

        // Assert
        horas.Should().Be(4.5m);
    }

    [Fact]
    public async Task VerificarTiempoPermanencia_TiempoSuficiente_DeberiaRetornarTrue()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var mockUserService = new Mock<UserService>(
            context,
            MockHelpers.CreateMockUserManager().Object,
            Mock.Of<Microsoft.AspNetCore.Http.IHttpContextAccessor>()
        );
        
        var service = new AsistenciaService(context, mockUserService.Object);
        var horaEntrada = new DateTime(2024, 1, 1, 8, 0, 0);
        var horaSalida = new DateTime(2024, 1, 1, 8, 15, 0); // 15 minutos después

        // Act
        var resultado = await service.VerificarTiempoPermanencia(horaEntrada, horaSalida);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task VerificarTiempoPermanencia_TiempoInsuficiente_DeberiaRetornarFalse()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var mockUserService = new Mock<UserService>(
            context,
            MockHelpers.CreateMockUserManager().Object,
            Mock.Of<Microsoft.AspNetCore.Http.IHttpContextAccessor>()
        );
        
        var service = new AsistenciaService(context, mockUserService.Object);
        var horaEntrada = new DateTime(2024, 1, 1, 8, 0, 0);
        var horaSalida = new DateTime(2024, 1, 1, 8, 5, 0); // Solo 5 minutos después

        // Act
        var resultado = await service.VerificarTiempoPermanencia(horaEntrada, horaSalida);

        // Assert
        resultado.Should().BeFalse();
    }
}

