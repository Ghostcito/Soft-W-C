#!/bin/bash
# Script para ejecutar pruebas en .NET 9
# Uso: ./scripts/run-tests.sh

echo "🧪 Ejecutando pruebas de SoftWC..."

# Ejecutar todas las pruebas
echo ""
echo "📋 Ejecutando todas las pruebas..."
dotnet test --verbosity normal

# Ejecutar pruebas con cobertura
echo ""
echo "📊 Generando reporte de cobertura..."
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=./TestResults/

# Ejecutar pruebas y generar reporte HTML
echo ""
echo "📄 Generando reporte HTML..."
dotnet test --logger "html;LogFileName=TestResults.html" --results-directory:./TestResults/

echo ""
echo "✅ Pruebas completadas!"
echo "📁 Reportes guardados en: ./TestResults/"

