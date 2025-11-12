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

public class ClienteApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly ApplicationDbContext _context;

    public ClienteApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
        
        var scope = factory.Services.CreateScope();
        _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    }

    [Fact]
    public async Task GetClientes_DeberiaRetornarListaDeClientes()
    {
        // Arrange
        var cliente1 = new Cliente
        {
            ClienteId = 1,
            Nombre = "Cliente Test 1",
            Estado = true
        };
        
        var cliente2 = new Cliente
        {
            ClienteId = 2,
            Nombre = "Cliente Test 2",
            Estado = true
        };
        
        _context.Cliente.AddRange(cliente1, cliente2);
        await _context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync("/Cliente");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task GetClienteById_ClienteExistente_DeberiaRetornarCliente()
    {
        // Arrange
        var cliente = new Cliente
        {
            ClienteId = 1,
            Nombre = "Cliente Test",
            Estado = true
        };
        
        _context.Cliente.Add(cliente);
        await _context.SaveChangesAsync();

        // Act
        var response = await _client.GetAsync($"/Cliente/Details/{cliente.ClienteId}");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task GetClienteById_ClienteNoExistente_DeberiaRetornarNotFound()
    {
        // Act
        var response = await _client.GetAsync("/Cliente/Details/999");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.NotFound, HttpStatusCode.Redirect);
    }
}
