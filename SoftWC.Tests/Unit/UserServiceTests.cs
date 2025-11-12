using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using SoftWC.Data;
using SoftWC.Models;
using SoftWC.Service;
using SoftWC.Tests.Helpers;
using Xunit;

namespace SoftWC.Tests.Unit;

public class UserServiceTests
{
    [Fact]
    public async Task FindByDniAsync_DniExistente_DeberiaRetornarUsuario()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var userManager = CreateMockUserManager();
        
        var usuario = new Usuario
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testuser",
            DNI = "12345678",
            Nombre = "Test",
            Apellido = "User"
        };
        
        context.Users.Add(usuario);
        await context.SaveChangesAsync();

        var httpContextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var service = new UserService(context, userManager.Object, httpContextAccessor.Object);

        // Act
        var resultado = await service.FindByDniAsync("12345678");

        // Assert
        resultado.Should().NotBeNull();
        resultado!.DNI.Should().Be("12345678");
        resultado.Nombre.Should().Be("Test");
    }

    [Fact]
    public async Task FindByDniAsync_DniNoExistente_DeberiaRetornarNull()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var userManager = CreateMockUserManager();
        var httpContextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var service = new UserService(context, userManager.Object, httpContextAccessor.Object);

        // Act
        var resultado = await service.FindByDniAsync("99999999");

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task IsDniAvailableAsync_DniDisponible_DeberiaRetornarTrue()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var userManager = CreateMockUserManager();
        var httpContextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var service = new UserService(context, userManager.Object, httpContextAccessor.Object);

        // Act
        var resultado = await service.IsDniAvailableAsync("99999999");

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task IsDniAvailableAsync_DniNoDisponible_DeberiaRetornarFalse()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var userManager = CreateMockUserManager();
        
        var usuario = new Usuario
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testuser",
            DNI = "12345678"
        };
        
        context.Users.Add(usuario);
        await context.SaveChangesAsync();

        var httpContextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var service = new UserService(context, userManager.Object, httpContextAccessor.Object);

        // Act
        var resultado = await service.IsDniAvailableAsync("12345678");

        // Assert
        resultado.Should().BeFalse();
    }

    private Mock<UserManager<Usuario>> CreateMockUserManager()
    {
        var store = new Mock<IUserStore<Usuario>>();
        return new Mock<UserManager<Usuario>>(
            store.Object,
            null!, null!, null!, null!, null!, null!, null!, null!);
    }
}

