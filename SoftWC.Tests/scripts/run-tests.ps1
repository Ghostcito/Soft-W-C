# Script para ejecutar pruebas en .NET 9
# Uso: .\scripts\run-tests.ps1

Write-Host "🧪 Ejecutando pruebas de SoftWC..." -ForegroundColor Cyan

# Ejecutar todas las pruebas
Write-Host "`n📋 Ejecutando todas las pruebas..." -ForegroundColor Yellow
dotnet test --verbosity normal

# Ejecutar pruebas con cobertura (requiere coverlet)
Write-Host "`n📊 Generando reporte de cobertura..." -ForegroundColor Yellow
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=./TestResults/

# Ejecutar pruebas y generar reporte HTML
Write-Host "`n📄 Generando reporte HTML..." -ForegroundColor Yellow
dotnet test --logger "html;LogFileName=TestResults.html" --results-directory:./TestResults/

Write-Host "`n✅ Pruebas completadas!" -ForegroundColor Green
Write-Host "📁 Reportes guardados en: ./TestResults/" -ForegroundColor Cyan

