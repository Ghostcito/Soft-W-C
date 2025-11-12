using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using SoftWC.Data;

namespace SoftWC.Tests.Integration;

/// <summary>
/// Factory personalizado para crear instancias de WebApplication para pruebas de integración.
/// Reemplaza PostgreSQL con InMemory Database para pruebas aisladas.
/// </summary>
public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // 1. REMOVER TODOS los servicios relacionados con DbContext y PostgreSQL
            RemoveDbContextServices(services);

            // 2. AGREGAR InMemory Database con nombre único
            var databaseName = $"TestDb_{Guid.NewGuid()}";
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseInMemoryDatabase(databaseName);
                options.EnableSensitiveDataLogging();
                options.EnableDetailedErrors();
            });

            // 3. VERIFICAR que InMemory está configurado correctamente
            var serviceProvider = services.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<ApplicationDbContext>();
                var logger = scopedServices.GetRequiredService<ILogger<CustomWebApplicationFactory>>();

                // Crear la base de datos en memoria
                db.Database.EnsureCreated();

                logger.LogInformation($"Test database '{databaseName}' created successfully");
            }
        });

        // Configurar entorno de pruebas
        builder.UseEnvironment("Test");
    }

    /// <summary>
    /// Remueve TODOS los servicios relacionados con DbContext y proveedores de BD de manera exhaustiva.
    /// Este método es crítico para evitar conflictos entre múltiples proveedores de base de datos.
    /// </summary>
    private void RemoveDbContextServices(IServiceCollection services)
    {
        var removedCount = 0;
        var removedTypes = new HashSet<string>();

        // FASE 1: Remover servicios específicos de DbContext por tipo exacto
        var exactTypesToRemove = new[]
        {
            typeof(DbContextOptions),
            typeof(DbContextOptions<>),
            typeof(DbContextOptions<ApplicationDbContext>),
            typeof(ApplicationDbContext),
            typeof(DbContext)
        };

        foreach (var type in exactTypesToRemove)
        {
            // Remover todas las instancias (puede haber múltiples registros)
            var descriptors = services
                .Where(d => d.ServiceType == type || 
                           (type.IsGenericTypeDefinition && 
                            d.ServiceType.IsGenericType && 
                            d.ServiceType.GetGenericTypeDefinition() == type))
                .ToList();

            foreach (var descriptor in descriptors)
            {
                services.Remove(descriptor);
                removedCount++;
                removedTypes.Add(descriptor.ServiceType.FullName ?? descriptor.ServiceType.Name);
            }
        }

        // FASE 2: Remover servicios relacionados con Npgsql/PostgreSQL
        // Buscar por nombre completo, namespace y tipo de implementación
        var postgresKeywords = new[] { "Npgsql", "PostgreSQL", "Postgres" };
        
        var postgresDescriptors = services
            .Where(d =>
            {
                // Verificar ServiceType
                var serviceTypeName = d.ServiceType.FullName ?? string.Empty;
                if (postgresKeywords.Any(keyword => serviceTypeName.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                    return true;

                // Verificar ImplementationType
                if (d.ImplementationType != null)
                {
                    var implTypeName = d.ImplementationType.FullName ?? string.Empty;
                    if (postgresKeywords.Any(keyword => implTypeName.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                        return true;
                }

                // Verificar ImplementationFactory (para servicios creados dinámicamente)
                if (d.ImplementationFactory != null)
                {
                    var factoryReturnType = d.ImplementationFactory.Method.ReturnType;
                    var factoryReturnTypeName = factoryReturnType.FullName ?? string.Empty;
                    if (postgresKeywords.Any(keyword => factoryReturnTypeName.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                        return true;

                    // Verificar parámetros del factory
                    var factoryParams = d.ImplementationFactory.Method.GetParameters();
                    foreach (var param in factoryParams)
                    {
                        var paramTypeName = param.ParameterType.FullName ?? string.Empty;
                        if (postgresKeywords.Any(keyword => paramTypeName.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                            return true;
                    }
                }

                // Verificar ImplementationInstance
                if (d.ImplementationInstance != null)
                {
                    var instanceTypeName = d.ImplementationInstance.GetType().FullName ?? string.Empty;
                    if (postgresKeywords.Any(keyword => instanceTypeName.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                        return true;
                }

                return false;
            })
            .ToList();

        foreach (var descriptor in postgresDescriptors)
        {
            services.Remove(descriptor);
            removedCount++;
            var typeName = descriptor.ServiceType.FullName ?? descriptor.ServiceType.Name;
            removedTypes.Add(typeName);
        }

        // FASE 3: Remover servicios de EntityFrameworkCore.Relational (compartidos entre proveedores)
        // Estos servicios pueden causar conflictos cuando se registran múltiples proveedores
        var relationalKeywords = new[] 
        { 
            "EntityFrameworkCore.Relational",
            "RelationalDatabaseProviderServices",
            "RelationalConnection",
            "RelationalCommand"
        };

        var relationalDescriptors = services
            .Where(d =>
            {
                var serviceTypeName = d.ServiceType.FullName ?? string.Empty;
                if (relationalKeywords.Any(keyword => serviceTypeName.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                    return true;

                if (d.ImplementationType != null)
                {
                    var implTypeName = d.ImplementationType.FullName ?? string.Empty;
                    if (relationalKeywords.Any(keyword => implTypeName.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
                        return true;
                }

                return false;
            })
            .ToList();

        foreach (var descriptor in relationalDescriptors)
        {
            // Solo remover si no es necesario para InMemory
            // InMemory no usa servicios relacionales, así que es seguro removerlos
            services.Remove(descriptor);
            removedCount++;
            var typeName = descriptor.ServiceType.FullName ?? descriptor.ServiceType.Name;
            removedTypes.Add(typeName);
        }

        // FASE 4: Remover servicios de configuración de base de datos específicos
        // Buscar servicios que puedan estar relacionados con la configuración de BD
        var dbConfigDescriptors = services
            .Where(d =>
            {
                var serviceTypeName = d.ServiceType.FullName ?? string.Empty;
                
                // Remover servicios relacionados con migraciones de PostgreSQL
                if (serviceTypeName.Contains("Migrations", StringComparison.OrdinalIgnoreCase) &&
                    (serviceTypeName.Contains("Npgsql", StringComparison.OrdinalIgnoreCase) ||
                     serviceTypeName.Contains("PostgreSQL", StringComparison.OrdinalIgnoreCase)))
                    return true;

                // Remover servicios de diseño de base de datos de PostgreSQL
                if (serviceTypeName.Contains("Design", StringComparison.OrdinalIgnoreCase) &&
                    (serviceTypeName.Contains("Npgsql", StringComparison.OrdinalIgnoreCase) ||
                     serviceTypeName.Contains("PostgreSQL", StringComparison.OrdinalIgnoreCase)))
                    return true;

                return false;
            })
            .ToList();

        foreach (var descriptor in dbConfigDescriptors)
        {
            services.Remove(descriptor);
            removedCount++;
            var typeName = descriptor.ServiceType.FullName ?? descriptor.ServiceType.Name;
            removedTypes.Add(typeName);
        }

        // LOG de servicios removidos (útil para debugging)
        // Nota: No construimos ServiceProvider aquí para evitar problemas de ciclo
        if (removedCount > 0)
        {
            // En modo debug, escribir a consola
            #if DEBUG
            Console.WriteLine($"[WebApplicationFactory] Removed {removedCount} database-related services");
            if (removedTypes.Count > 0)
            {
                var typesPreview = string.Join(", ", removedTypes.Take(5));
                Console.WriteLine($"  Removed types: {typesPreview}");
                if (removedTypes.Count > 5)
                    Console.WriteLine($"  ... and {removedTypes.Count - 5} more");
            }
            #endif
        }
    }

    /// <summary>
    /// Crea un cliente HTTP configurado para pruebas
    /// </summary>
    public new HttpClient CreateClient()
    {
        var client = base.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false,
            BaseAddress = new Uri("http://localhost")
        });

        return client;
    }
}

