#!/bin/bash
# Script para ejecutar pruebas en .NET 9
# Uso: ./scripts/run-tests.sh [--coverage] [--html]

# Cambiar al directorio del proyecto de pruebas
SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
TEST_PROJECT_DIR="$(dirname "$SCRIPT_DIR")"
cd "$TEST_PROJECT_DIR" || exit 1

echo "🧪 Ejecutando pruebas de SoftWC..."
echo "📁 Directorio: $TEST_PROJECT_DIR"
echo ""

# Crear directorio de resultados si no existe
RESULTS_DIR="$TEST_PROJECT_DIR/TestResults"
mkdir -p "$RESULTS_DIR"

# Ejecutar todas las pruebas con verbosidad normal
echo "📋 Ejecutando todas las pruebas..."
dotnet test --verbosity normal --logger "trx;LogFileName=TestResults.trx" --results-directory:"$RESULTS_DIR"

TEST_EXIT_CODE=$?

if [ $TEST_EXIT_CODE -ne 0 ]; then
    echo "⚠️  Algunas pruebas fallaron"
else
    echo "✅ Todas las pruebas pasaron"
fi

# Ejecutar pruebas con cobertura (si se solicita)
if [[ "$*" == *"--coverage"* ]]; then
    echo ""
    echo "📊 Generando reporte de cobertura..."
    dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput="$RESULTS_DIR/" --results-directory:"$RESULTS_DIR"
fi

# Ejecutar pruebas y generar reporte HTML (si se solicita)
if [[ "$*" == *"--html"* ]]; then
    echo ""
    echo "📄 Generando reporte HTML..."
    dotnet test --logger "html;LogFileName=TestResults.html" --results-directory:"$RESULTS_DIR"
fi

echo ""
echo "✅ Pruebas completadas!"
echo "📁 Reportes guardados en: $RESULTS_DIR"
echo ""
echo "💡 Tip: Usa --coverage para generar reporte de cobertura"
echo "💡 Tip: Usa --html para generar reporte HTML"

