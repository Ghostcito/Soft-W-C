# Work and Cleaning Services SAC

Proyecto empresa Work and Cleaning SAC - Sistema de gestión de servicios de limpieza y trabajo.

## 🛠️ Tecnologías

- **.NET 9.0** - Framework principal
- **ASP.NET Core MVC** - Framework web
- **Entity Framework Core** - ORM
- **PostgreSQL** - Base de datos (producción)
- **SQLite** - Base de datos (desarrollo local)
- **ASP.NET Identity** - Autenticación y autorización
- **xUnit** - Framework de pruebas
- **Moq** - Mocking para pruebas
- **FluentAssertions** - Aserciones legibles

## 📋 Requisitos Previos

### Herramientas Necesarias

1. **.NET SDK 9.0**
   ```bash
   dotnet --version  # Debe mostrar 9.0.x
   ```

2. **Entity Framework Core Tools**
   ```bash
   dotnet tool update --global dotnet-ef --version 7.0.3
   ```

3. **PostgreSQL** (para producción) o **SQLite** (para desarrollo)

## 🚀 Configuración Inicial

### 1. Actualizar Herramientas

```bash
dotnet tool update --global dotnet-ef --version 7.0.3
```

### 2. Restaurar Dependencias

```bash
dotnet restore
```

### 3. Configurar Base de Datos

Editar `appsettings.json` con tu cadena de conexión:

```json
{
  "ConnectionStrings": {
    "PostgreSQLConnection": "Host=...;Database=...;Username=...;Password=..."
  }
}
```

### 4. Aplicar Migraciones

```bash
dotnet ef database update
```

## 🧪 Pruebas

### Ejecutar Todas las Pruebas

```powershell
# Windows
cd SoftWC.Tests
.\scripts\run-tests.ps1

# Con reporte HTML
.\scripts\run-tests.ps1 -HtmlReport
```

```bash
# Linux/Mac
cd SoftWC.Tests
./scripts/run-tests.sh

# Con reporte HTML
./scripts/run-tests.sh --html
```

### Comando Directo

```bash
dotnet test --logger "html;LogFileName=TestResults.html" --results-directory:./TestResults/
```

### Ver Reportes

Los reportes se generan en `SoftWC.Tests/TestResults/`:

```powershell
# Abrir reporte HTML
start SoftWC.Tests\TestResults\TestResults.html
```

## 📊 Alcance de Pruebas

El proyecto incluye **43 pruebas automatizadas**:

- **21 Pruebas Unitarias** (48.8%) - Lógica de negocio
- **4 Pruebas Integración BD** (9.3%) - Operaciones con base de datos
- **11 Pruebas Integración API** (25.6%) - Endpoints HTTP
- **7 Pruebas Rendimiento** (16.3%) - Tiempos de ejecución

**Cobertura General: ~82%**

### Módulos Cubiertos

- ✅ Asistencia (90% cobertura)
- ✅ Usuarios/Auth (85% cobertura)
- ✅ Validación GPS (95% cobertura)
- ✅ Exportación (80% cobertura)
- ✅ Clientes (75% cobertura)
- ✅ Sedes (70% cobertura)
- ⚠️ Turnos (50% cobertura)

## 🛠️ Herramientas de Pruebas Utilizadas

### Incluidas en el Proyecto

| Herramienta | Versión | Propósito |
|-------------|---------|-----------|
| **xUnit** | 2.9.2 | Framework de pruebas |
| **Moq** | 4.20.72 | Mocking de dependencias |
| **FluentAssertions** | 8.8.0 | Aserciones legibles |
| **EF Core InMemory** | 9.0.0 | Base de datos en memoria |
| **ASP.NET Core Testing** | 9.0.0 | Pruebas de integración API |

### Opcionales (Para Pruebas Externas)

- **K6**: Pruebas de carga (scripts en `tests/k6/`)
- **OWASP ZAP**: Escaneo de seguridad (scripts en `tests/zap/`)

## 📁 Estructura del Proyecto

```
Soft-W-C/
├── Controllers/          # Controladores MVC
├── Models/               # Modelos de datos
├── Service/              # Lógica de negocio
├── Data/                 # Contexto de base de datos y migraciones
├── Views/                # Vistas Razor
├── SoftWC.Tests/         # Proyecto de pruebas
│   ├── Unit/            # Pruebas unitarias
│   ├── Integration/     # Pruebas de integración
│   └── scripts/         # Scripts de ejecución
├── tests/               # Directorio reservado para futuras pruebas externas
├── results/             # Reportes de pruebas externas
└── scripts/             # Scripts de utilidad
```

## 🔧 Comandos Útiles

### Desarrollo

```bash
# Ejecutar la aplicación
dotnet run

# Compilar
dotnet build

# Limpiar
dotnet clean
```

### Base de Datos

```bash
# Crear migración
dotnet ef migrations add NombreMigracion -o Data/Migrations

# Aplicar migraciones
dotnet ef database update

# Revertir última migración
dotnet ef database update NombreMigracionAnterior
```

### Generación de Código

```bash
# Generar CRUD para una entidad
dotnet aspnet-codegenerator controller -name ClienteController -m Cliente -dc ApplicationDbContext --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries

# Generar vistas de Identity
dotnet aspnet-codegenerator identity -dc ApplicationDbContext --files "Account.Register;Account.Login"
```

### Pruebas

```bash
# Ejecutar todas las pruebas
dotnet test

# Ejecutar con verbosidad
dotnet test --verbosity normal

# Ejecutar con reporte HTML
dotnet test --logger "html;LogFileName=TestResults.html" --results-directory:./TestResults/

# Filtrar pruebas
dotnet test --filter "FullyQualifiedName~Unit"
```

## 📚 Documentación

- `PLAN_DE_PRUEBAS.md` - Plan completo de pruebas y cobertura
- `COMO_VER_REPORTES.md` - Cómo ver los reportes HTML
- `GUIA_EJECUCION_PRUEBAS.md` - Guía de ejecución (para pruebas externas)
- `SoftWC.Tests/scripts/README.md` - Scripts de pruebas
- `tests/README.md` - Información sobre pruebas

## 🐳 Docker

El proyecto incluye un `Dockerfile` para contenedorización:

```bash
# Construir imagen
docker build -t softwc-api .

# Ejecutar contenedor
docker run -p 8080:8080 softwc-api
```

## 📝 Notas

- El proyecto utiliza **PostgreSQL** en producción y **SQLite** en desarrollo local
- Las pruebas utilizan **Entity Framework Core InMemory** para aislamiento
- Los reportes de pruebas se generan en `SoftWC.Tests/TestResults/`
- La carpeta `results/` está reservada para futuros reportes externos

## 🔗 Enlaces Útiles

- [Documentación .NET 9](https://learn.microsoft.com/dotnet/)
- [Documentación ASP.NET Core](https://learn.microsoft.com/aspnet/core/)
- [Documentación Entity Framework Core](https://learn.microsoft.com/ef/core/)
- [Documentación xUnit](https://xunit.net/)
