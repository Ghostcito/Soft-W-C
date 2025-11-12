using FluentAssertions;
using SoftWC.Models.ViewModels;
using SoftWC.Service.Implementation;
using Xunit;

namespace SoftWC.Tests.Unit;

public class ExcelExportServiceTests
{
    [Fact]
    public void GenerateExcel_DeberiaGenerarArchivoExcel()
    {
        // Arrange
        var service = new ExcelExportService();
        var data = new List<ResumenPagoVM>
        {
            new ResumenPagoVM
            {
                EmpleadoId = "1",
                Empleado = "Juan Pérez",
                DNI = "12345678",
                TotalHoras = 40m,
                PrecioBase = 15.50m,
                TotalPago = 620m,
                Servicios = new List<string> { "Limpieza", "Mantenimiento" }
            },
            new ResumenPagoVM
            {
                EmpleadoId = "2",
                Empleado = "María García",
                DNI = "87654321",
                TotalHoras = 35m,
                PrecioBase = 18.00m,
                TotalPago = 630m,
                Servicios = new List<string> { "Limpieza" }
            }
        };
        
        var año = 2024;
        var mes = 1;
        var quincena = 1;
        
        // Act
        var resultado = service.GenerateExcel(data, año, mes, quincena);
        
        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().NotBeEmpty();
        resultado.Length.Should().BeGreaterThan(0);
    }
    
    [Fact]
    public void GenerateExcel_ConListaVacia_DeberiaGenerarExcelConEncabezados()
    {
        // Arrange
        var service = new ExcelExportService();
        var data = new List<ResumenPagoVM>();
        var año = 2024;
        var mes = 1;
        var quincena = 1;
        
        // Act
        var resultado = service.GenerateExcel(data, año, mes, quincena);
        
        // Assert
        resultado.Should().NotBeNull();
        resultado.Should().NotBeEmpty();
        // El archivo debería tener al menos los encabezados
        resultado.Length.Should().BeGreaterThan(0);
    }
    
    [Fact]
    public void GenerateExcel_ConDatosComplejos_DeberiaGenerarExcelCorrectamente()
    {
        // Arrange
        var service = new ExcelExportService();
        var data = new List<ResumenPagoVM>
        {
            new ResumenPagoVM
            {
                EmpleadoId = "1",
                Empleado = "Test User",
                DNI = "12345678",
                TotalHoras = 160m,
                PrecioBase = 20.00m,
                TotalPago = 3200m,
                Servicios = new List<string> { "Servicio 1", "Servicio 2", "Servicio 3" }
            }
        };
        
        var año = 2024;
        var mes = 12;
        var quincena = 2;
        
        // Act
        var resultado = service.GenerateExcel(data, año, mes, quincena);
        
        // Assert
        resultado.Should().NotBeNull();
        resultado.Length.Should().BeGreaterThan(100); // Debe tener contenido significativo
    }
}

