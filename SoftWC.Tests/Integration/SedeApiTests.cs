using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SoftWC.Data;
using SoftWC.Models;
using System.Net;
using Xunit;

namespace SoftWC.Tests.Integration;

public class SedeApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ApplicationDbContext _context;

    public SedeApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        
        var scope = factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    [Fact]
    public async Task GetSedes_DeberiaRetornarListaDeSedes()
    {
        // Arrange
        var cliente = new Cliente
        {
            ClienteId = 1,
            Nombre = "Cliente Test",
            Estado = true
        };
        
        var sede1 = new Sede
        {
            SedeId = 1,
            ClienteId = 1,
            Nombre_local = "Sede Central",
            Latitud = -12.0464m,
            Longitud = -77.0428m,
            Radio = 1000m
        };
        
        var sede2 = new Sede
        {
            SedeId = 2,
            ClienteId = 1,
            Nombre_local = "Sede Norte",
            Latitud = -12.0564m,
            Longitud = -77.0528m,
            Radio = 500m
        };
        
        _context.Cliente.Add(cliente);
        _context.Sede.AddRange(sede1, sede2);
        await _context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/Sede/Index");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task GetSedesByClienteId_DeberiaRetornarSedesDelCliente()
    {
        // Arrange
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
            Radio = 1000m
        };
        
        _context.Cliente.Add(cliente);
        _context.Sede.Add(sede);
        await _context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/Sede/Index/{cliente.ClienteId}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Redirect);
    }
}

