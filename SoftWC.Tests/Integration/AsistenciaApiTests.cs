using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SoftWC.Data;
using SoftWC.Models;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace SoftWC.Tests.Integration;

public class AsistenciaApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ApplicationDbContext _context;

    public AsistenciaApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        
        // Obtener el DbContext del factory
        var scope = factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    [Fact]
    public async Task GetAsistencias_DeberiaRetornarListaDeAsistencias()
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
        
        var asistencia = new Asistencia
        {
            IdEmpleado = usuario.Id,
            Fecha = DateTime.UtcNow.Date,
            HoraEntrada = DateTime.UtcNow,
            Presente = true
        };
        
        _context.Asistencia.Add(asistencia);
        await _context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/Asistencia");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetAsistenciaById_AsistenciaExistente_DeberiaRetornarAsistencia()
    {
        // Arrange
        var usuario = new Usuario
        {
            Id = Guid.NewGuid().ToString(),
            UserName = "testuser",
            DNI = "12345678"
        };
        
        _context.Users.Add(usuario);
        
        var asistencia = new Asistencia
        {
            IdEmpleado = usuario.Id,
            Fecha = DateTime.UtcNow.Date,
            HoraEntrada = DateTime.UtcNow,
            Presente = true
        };
        
        _context.Asistencia.Add(asistencia);
        await _context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/Asistencia/Details/{asistencia.IdAsistencia}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Redirect);
    }
}

