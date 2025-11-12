# Script para ejecutar pruebas en .NET 9
param(
    [switch]$Coverage,
    [switch]$HtmlReport
)

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$TestProjectDir = Split-Path -Parent $ScriptDir
Set-Location $TestProjectDir

Write-Host "🧪 Ejecutando pruebas de SoftWC..." -ForegroundColor Cyan
Write-Host "📁 Directorio: $TestProjectDir" -ForegroundColor Gray
Write-Host ""

$resultsDir = Join-Path $TestProjectDir "TestResults"
if (-not (Test-Path $resultsDir)) {
    New-Item -ItemType Directory -Path $resultsDir -Force | Out-Null
}

$testCommand = "dotnet test --verbosity normal --logger `"trx;LogFileName=TestResults.trx`" --results-directory:`"$resultsDir`""

if ($HtmlReport) {
    $testCommand += " --logger `"html;LogFileName=TestResults.html`""
}

if ($Coverage) {
    $testCommand += " /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=`"$resultsDir/`""
}

Write-Host "📋 Ejecutando todas las pruebas..." -ForegroundColor Yellow
Invoke-Expression $testCommand

if ($LASTEXITCODE -ne 0) {
    Write-Host ""
    Write-Host "⚠️  Algunas pruebas fallaron" -ForegroundColor Yellow
} else {
    Write-Host ""
    Write-Host "✅ Todas las pruebas pasaron" -ForegroundColor Green
}

Write-Host ""
Write-Host "✅ Pruebas completadas!" -ForegroundColor Green
Write-Host "📁 Reportes guardados en: $resultsDir" -ForegroundColor Cyan
Write-Host ""
Write-Host "💡 Tip: Usa -Coverage para generar reporte de cobertura" -ForegroundColor Gray
Write-Host "💡 Tip: Usa -HtmlReport para generar reporte HTML" -ForegroundColor Gray

if ($HtmlReport) {
    $htmlPath = Join-Path $resultsDir "TestResults.html"
    $exists = Test-Path $htmlPath
    if ($exists) {
        Write-Host ""
        Write-Host "📊 Abriendo reporte HTML..." -ForegroundColor Cyan
        Start-Process $htmlPath
    }
}
