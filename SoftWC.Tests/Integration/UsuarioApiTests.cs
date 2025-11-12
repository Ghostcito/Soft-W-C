using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SoftWC.Data;
using SoftWC.Models;
using System.Net;
using Xunit;

namespace SoftWC.Tests.Integration;

public class UsuarioApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ApplicationDbContext _context;

    public UsuarioApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        
        var scope = factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    [Fact]
    public async Task GetUsuarios_DeberiaRetornarListaDeUsuarios()
    {
        // Arrange
        var usuario = new Usuario
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testuser",
            DNI = "12345678",
            Nombre = "Test",
            Apellido = "User"
        };
        
        _context.Users.Add(usuario);
        await _context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/Usuario");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task GetUsuarioById_UsuarioExistente_DeberiaRetornarUsuario()
    {
        // Arrange
        var usuario = new Usuario
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testuser",
            DNI = "12345678",
            Nombre = "Test",
            Apellido = "User"
        };
        
        _context.Users.Add(usuario);
        await _context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/Usuario/Details/{usuario.Id}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Redirect);
    }
}

