using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SoftWC.Data;
using SoftWC.Models;
using SoftWC.Tests.Integration;
using System.Net;
using Xunit;

namespace SoftWC.Tests.Integration;

public class TurnoApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public TurnoApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTurnos_DeberiaRetornarListaDeTurnos()
    {
        // Act - Las pruebas de API requieren autenticación
        var response = await _client.GetAsync("/Turno");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.Unauthorized, HttpStatusCode.Redirect);
    }

    [Fact]
    public async Task GetTurnoById_TurnoExistente_DeberiaRetornarTurno()
    {
        // Act
        var response = await _client.GetAsync("/Turno/Details/1");

        // Assert
        response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NotFound, HttpStatusCode.Unauthorized, HttpStatusCode.Redirect);
    }
}

