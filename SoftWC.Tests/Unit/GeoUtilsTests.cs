using FluentAssertions;
using Prueba_Geolocalizacion.Utils;
using Xunit;

namespace SoftWC.Tests.Unit;

public class GeoUtilsTests
{
    [Fact]
    public void CalcularDistancia_DeberiaRetornarDistanciaCorrecta()
    {
        // Arrange
        // Coordenadas de Lima, Perú (aproximadamente)
        double lat1 = -12.0464;
        double lon1 = -77.0428;
        
        // Coordenadas de un punto cercano (aproximadamente 1 km de distancia)
        double lat2 = -12.0554;
        double lon2 = -77.0428;

        // Act
        var distancia = GeoUtils.CalcularDistancia(lat1, lon1, lat2, lon2);

        // Assert
        distancia.Should().BeGreaterThan(0);
        distancia.Should().BeApproximately(1000, 100); // Aproximadamente 1 km con margen de 100m
    }

    [Fact]
    public void CalcularDistancia_CoordenadasIguales_DeberiaRetornarCero()
    {
        // Arrange
        double lat = -12.0464;
        double lon = -77.0428;

        // Act
        var distancia = GeoUtils.CalcularDistancia(lat, lon, lat, lon);

        // Assert
        distancia.Should().Be(0);
    }

    [Fact]
    public void CalcularDistancia_DistanciaGrande_DeberiaCalcularCorrectamente()
    {
        // Arrange
        // Lima, Perú
        double lat1 = -12.0464;
        double lon1 = -77.0428;
        
        // Cusco, Perú (aproximadamente 570 km)
        double lat2 = -13.5319;
        double lon2 = -71.9675;

        // Act
        var distancia = GeoUtils.CalcularDistancia(lat1, lon1, lat2, lon2);

        // Assert
        distancia.Should().BeGreaterThan(500000); // Más de 500 km
        distancia.Should().BeLessThan(600000); // Menos de 600 km
    }
}

