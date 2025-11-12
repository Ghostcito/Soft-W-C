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

public class EmpleadoServiceTests
{
    [Fact]
    public async Task GetEmpleados_Administrador_DeberiaRetornarEmpleadosSinAdministradores()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var userManager = MockHelpers.CreateMockUserManager();
        
        var admin = new Usuario
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "admin",
            DNI = "11111111",
            Nombre = "Admin",
            Apellido = "User"
        };
        
        var empleado1 = new Usuario
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "empleado1",
            DNI = "22222222",
            Nombre = "Empleado",
            Apellido = "Uno"
        };
        
        var empleado2 = new Usuario
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "empleado2",
            DNI = "33333333",
            Nombre = "Empleado",
            Apellido = "Dos"
        };
        
        context.Users.AddRange(admin, empleado1, empleado2);
        await context.SaveChangesAsync();
        
        // Configurar UserManager para retornar roles
        userManager.Setup(um => um.GetRolesAsync(It.IsAny<Usuario>()))
            .ReturnsAsync((Usuario u) => 
            {
                if (u.UserName == "admin") return new List<string> { "Administrador" };
                return new List<string> { "Empleado" };
            });
        
        var httpContextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var userService = new MockUserServiceForEmpleado(context, admin.Id, userManager.Object, httpContextAccessor.Object);
        var empleadoService = new EmpleadoService(context, userManager.Object, httpContextAccessor.Object, userService);
        
        // Act
        var resultado = await empleadoService.GetEmpleados();
        
        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().NotContain(u => u.UserName == "admin");
        resultado.Should().HaveCount(2);
    }
    
    [Fact]
    public async Task GetEmpleadosOfSuper_DeberiaRetornarEmpleadosDelSupervisor()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var userManager = MockHelpers.CreateMockUserManager();
        
        var supervisor = new Usuario
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "supervisor",
            DNI = "44444444"
        };
        
        var empleado1 = new Usuario
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "empleado1",
            DNI = "55555555"
        };
        
        var empleado2 = new Usuario
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "empleado2",
            DNI = "66666666"
        };
        
        context.Users.AddRange(supervisor, empleado1, empleado2);
        
        var supervision1 = new Supervision
        {
            SupervisorId = supervisor.Id,
            EmpleadoId = empleado1.Id
        };
        
        var supervision2 = new Supervision
        {
            SupervisorId = supervisor.Id,
            EmpleadoId = empleado2.Id
        };
        
        context.Supervision.AddRange(supervision1, supervision2);
        await context.SaveChangesAsync();
        
        var httpContextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var userService = new MockUserServiceForEmpleado(context, supervisor.Id, userManager.Object, httpContextAccessor.Object);
        var empleadoService = new EmpleadoService(context, userManager.Object, httpContextAccessor.Object, userService);
        
        // Act
        var resultado = await empleadoService.GetEmpleadosOfSuper(supervisor.Id);
        
        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().HaveCount(2);
        resultado.Should().Contain(u => u.Id == empleado1.Id);
        resultado.Should().Contain(u => u.Id == empleado2.Id);
    }
    
    [Fact]
    public async Task ObtenerEmpleadosAsync_DeberiaRetornarSoloEmpleados()
    {
        // Arrange
        using var context = TestDbContextFactory.CreateInMemoryContext();
        var userManager = MockHelpers.CreateMockUserManager();
        
        var empleado = new Usuario
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "empleado1",
            DNI = "77777777",
            Apellido = "Zapata",
            Nombre = "Juan"
        };
        
        context.Users.Add(empleado);
        await context.SaveChangesAsync();
        
        // Crear rol de Empleado
        var role = new IdentityRole { Id = Guid.NewGuid().ToString(), Name = "Empleado" };
        context.Roles.Add(role);
        
        // Asignar rol al empleado
        var userRole = new IdentityUserRole<string>
        {
            UserId = empleado.Id,
            RoleId = role.Id
        };
        context.UserRoles.Add(userRole);
        await context.SaveChangesAsync();
        
        var httpContextAccessor = new Mock<Microsoft.AspNetCore.Http.IHttpContextAccessor>();
        var userService = new MockUserServiceForEmpleado(context, empleado.Id, userManager.Object, httpContextAccessor.Object);
        var empleadoService = new EmpleadoService(context, userManager.Object, httpContextAccessor.Object, userService);
        
        // Act
        var resultado = await empleadoService.ObtenerEmpleadosAsync();
        
        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().Contain(u => u.Id == empleado.Id);
    }
}

// Helper para crear MockUserService para EmpleadoService
public class MockUserServiceForEmpleado : UserService
{
    private readonly ApplicationDbContext _testContext;
    private readonly string _userId;

    public MockUserServiceForEmpleado(
        ApplicationDbContext context,
        string userId,
        UserManager<Usuario> userManager,
        Microsoft.AspNetCore.Http.IHttpContextAccessor httpContextAccessor)
        : base(context, userManager, httpContextAccessor)
    {
        _testContext = context;
        _userId = userId;
    }

    public override async Task<Usuario> GetCurrentUserAsync()
    {
        return await _testContext.Users.FindAsync(_userId)
            ?? throw new InvalidOperationException("Usuario no encontrado");
    }
}

